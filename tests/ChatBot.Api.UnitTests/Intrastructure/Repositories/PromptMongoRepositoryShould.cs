using ChatBot.Api.Infrastructure.Repositories;
using Common.Mongo.Models;
using AutoFixture;
using Common.Mongo;
using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using Microsoft.Extensions.Logging;
using ChatBot.Api.Domain.Exceptions;
using ChatBot.Api.Domain.PromptEntity;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Models;

namespace ChatBot.Api.UnitTests.Infrastructure.Repositories;

public class PromptMongoRepositoryShould
{
    private readonly ILogger<PromptMongoRepository> _logger;
    private readonly IMongoClientFactory _mongoClientFactory;
    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IMongoCollection<PromptDal> _mongoCollection;
    private readonly IAsyncCursor<PromptDal> _mongoCursor;

    private readonly PromptMongoRepository _sut;
    private readonly IFixture _fixture;

    public PromptMongoRepositoryShould()
    {
        _logger = Substitute.For<ILogger<PromptMongoRepository>>();
        _mongoClientFactory = Substitute.For<IMongoClientFactory>();
        _mongoClient = Substitute.For<IMongoClient>();
        _mongoDatabase = Substitute.For<IMongoDatabase>();
        _mongoCollection = Substitute.For<IMongoCollection<PromptDal>>();
        _mongoCursor = Substitute.For<IAsyncCursor<PromptDal>>();

        _mongoClientFactory.GetMongoConfigurationRecord().Returns(new MongoConfigurationRecord());
        _mongoClientFactory.CreateClient().Returns(_mongoClient);
        _mongoClient.GetDatabase(Arg.Any<string>()).Returns(_mongoDatabase);
        _mongoDatabase.GetCollection<PromptDal>(Arg.Any<string>(), Arg.Any<MongoCollectionSettings>()).Returns(_mongoCollection);
        _mongoCollection.FindAsync<PromptDal>(Arg.Any<FilterDefinition<PromptDal>>(), Arg.Any<FindOptions<PromptDal>>(), Arg.Any<CancellationToken>())
            .Returns(_mongoCursor);

        _sut = new PromptMongoRepository(_logger, _mongoClientFactory);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAsync_WithPromptId_ShouldReturnPrompt()
    {
        // Arrange
        var promptDal = _fixture.Create<PromptDal>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns(new PromptDal[] { promptDal });

        // Act
        var result = await _sut.GetAsync(promptDal.Owner, promptDal.PromptId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.PromptId.Should().Be(promptDal.PromptId);
        result!.Key.Should().Be(promptDal.Key);
        result!.Value.Should().Be(promptDal.Value);
        result!.Owner.Should().Be(promptDal.Owner);
    }

    [Fact]
    public async Task GetAsync_WithKey_ShouldReturnPrompt()
    {
        // Arrange
        var promptDal = _fixture.Create<PromptDal>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns(new PromptDal[] { promptDal });

        // Act
        var result = await _sut.GetAsync(promptDal.Owner, promptDal.Key, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.PromptId.Should().Be(promptDal.PromptId);
        result!.Key.Should().Be(promptDal.Key);
        result!.Value.Should().Be(promptDal.Value);
        result!.Owner.Should().Be(promptDal.Owner);
    }

    [Fact]
    public async Task GetManyAsync_ShouldReturnPrompts()
    {
        // Arrange
        var promptDals = _fixture.CreateMany<PromptDal>(count: 1).ToList();
        var cancellationToken = CancellationToken.None;

        // It is necessary to return true then false or the loop will never end
        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(true, false);
        _mongoCursor.Current.Returns(promptDals);

        // Act
        var result = await _sut.GetManyAsync(promptDals.First().Owner, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(promptDals.Count);

        var firstPromptDal = promptDals.First();
        var firstPrompt = result.First();

        firstPrompt.PromptId.Should().Be(firstPromptDal.PromptId);
        firstPrompt.Key.Should().Be(firstPromptDal.Key);
        firstPrompt.Value.Should().Be(firstPromptDal.Value);
        firstPrompt.Owner.Should().Be(firstPromptDal.Owner);
    }

    [Fact]
    public async Task SaveAsync_ShouldInsertPrompt_WhenPromptDoesNotExist()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(false);

        // Act
        await _sut.SaveAsync(prompt, cancellationToken);

        // Assert
        await _mongoCollection.Received(1).InsertOneAsync(
            Arg.Is<PromptDal>(x =>
                x.PromptId == prompt.PromptId &&
                x.Key == prompt.Key &&
                x.Value == prompt.Value &&
                x.Owner == prompt.Owner),
            Arg.Any<InsertOneOptions>(),
            cancellationToken);
        await _mongoCollection.DidNotReceive().ReplaceOneAsync(Arg.Any<FilterDefinition<PromptDal>>(), Arg.Any<PromptDal>(), Arg.Any<ReplaceOptions>(), cancellationToken);
    }

    [Fact]
    public async Task SaveAsync_ShouldUpdatePrompt_WhenPromptExists()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var storedPromptDal = new PromptDal
        {
            PromptId = prompt.PromptId,
            Key = _fixture.Create<string>(),
            Value = _fixture.Create<string>(),
            Owner = prompt.Owner
        };

        _mongoCursor.MoveNextAsync(CancellationToken.None).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns(new PromptDal[] { storedPromptDal });

        // Act
        await _sut.SaveAsync(prompt, CancellationToken.None);

        // Assert
        await _mongoCollection.DidNotReceive().InsertOneAsync(Arg.Any<PromptDal>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
        await _mongoCollection.Received(1).ReplaceOneAsync(
                Arg.Any<FilterDefinition<PromptDal>>(),
                Arg.Is<PromptDal>(
                    x => x.PromptId == prompt.PromptId &&
                         x.Key == prompt.Key &&
                         x.Value == prompt.Value &&
                         x.Owner == prompt.Owner
                ),
                Arg.Is<ReplaceOptions>(x => x.IsUpsert == false),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_ShouldThrowPromptAuthorizationException_WhenPromptOwnerIsDifferent()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var storedPromptDal = new PromptDal()
        {
            PromptId = prompt.PromptId,
            Key = prompt.Key,
            Value = prompt.Value,
            Owner = "DifferentOwner"
        };

        _mongoCursor.MoveNextAsync(CancellationToken.None).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns(new PromptDal[] { storedPromptDal });

        // Act
        Func<Task> act = async () => await _sut.SaveAsync(prompt, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<PromptAuthorizationException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePrompt_WhenFoundAndOwnedByRequest()
    {
        // Arrange
        var prompt = _fixture.Create<PromptDal>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns(new PromptDal[] { prompt });

        _mongoCollection
            .DeleteOneAsync(Arg.Any<FilterDefinition<PromptDal>>(), cancellationToken)
            .Returns(new DeleteResult.Acknowledged(1));

        // Act
        var act = () => _sut.DeleteAsync(prompt.Owner, prompt.PromptId, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotThrow_WhenPromptNotFound()
    {
        // Arrange
        var prompt = _fixture.Create<PromptDal>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(false);

        // Act
        var act = () => _sut.DeleteAsync(prompt.Owner, prompt.PromptId, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowPromptAuthorizationException_WhenPromptOwnerIsDifferent()
    {
        // Arrange
        var prompt = _fixture.Create<PromptDal>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns(new PromptDal[] { prompt });

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync("DifferentOwner", prompt.PromptId, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<PromptAuthorizationException>();
    }
}
