using AutoFixture;
using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo.Mappers;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo.Models;
using Common.Mongo;
using Common.Mongo.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NSubstitute;

namespace ChatBot.UnitTests.Infrastructure.Repositories;

public class PromptMongoRepositoryShould
{
    private readonly IMongoCollection<PromptDal> _mongoCollection;
    private readonly IAsyncCursor<PromptDal> _mongoCursor;

    private readonly PromptMongoRepository _sut;
    private readonly IFixture _fixture;

    public PromptMongoRepositoryShould()
    {
        var logger = Substitute.For<ILogger<PromptMongoRepository>>();
        var mongoClientFactory = Substitute.For<IMongoClientFactory>();
        var mongoClient = Substitute.For<IMongoClient>();
        var mongoDatabase = Substitute.For<IMongoDatabase>();
        _mongoCollection = Substitute.For<IMongoCollection<PromptDal>>();
        _mongoCursor = Substitute.For<IAsyncCursor<PromptDal>>();

        mongoClientFactory.GetMongoConfigurationRecord().Returns(new MongoConfigurationRecord());
        mongoClientFactory.CreateClient().Returns(mongoClient);
        mongoClient.GetDatabase(Arg.Any<string>()).Returns(mongoDatabase);
        mongoDatabase.GetCollection<PromptDal>(Arg.Any<string>(), Arg.Any<MongoCollectionSettings>()).Returns(_mongoCollection);
        _mongoCollection.FindAsync<PromptDal>(Arg.Any<FilterDefinition<PromptDal>>(), Arg.Any<FindOptions<PromptDal>>(), Arg.Any<CancellationToken>())
            .Returns(_mongoCursor);

        _sut = new PromptMongoRepository(logger, mongoClientFactory);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAsync_WithPromptId_ShouldReturnPrompt()
    {
        // Arrange
        var promptDal = _fixture.Create<PromptDal>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns([promptDal]);

        // Act
        var result = await _sut.GetAsync(promptDal.PromptId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.PromptId.Should().Be(promptDal.PromptId);
        result!.Key.Should().Be(promptDal.Key);
        result!.Value.Should().Be(promptDal.Value);
        result!.OwnerId.Should().Be(promptDal.OwnerId);
    }

    [Fact]
    public async Task GetAsync_WithKey_ShouldReturnPrompt()
    {
        // Arrange
        var promptDal = _fixture.Create<PromptDal>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns([promptDal]);

        // Act
        var result = await _sut.GetAsync(promptDal.OwnerId, promptDal.Key, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.PromptId.Should().Be(promptDal.PromptId);
        result!.Key.Should().Be(promptDal.Key);
        result!.Value.Should().Be(promptDal.Value);
        result!.OwnerId.Should().Be(promptDal.OwnerId);
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
        var result = await _sut.GetManyAsync(promptDals.First().OwnerId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(promptDals.Count);

        var firstPromptDal = promptDals.First();
        var firstPrompt = result.First();

        firstPrompt.PromptId.Should().Be(firstPromptDal.PromptId);
        firstPrompt.Key.Should().Be(firstPromptDal.Key);
        firstPrompt.Value.Should().Be(firstPromptDal.Value);
        firstPrompt.OwnerId.Should().Be(firstPromptDal.OwnerId);
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertPrompt_WhenPromptDoesNotExist()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(false);

        // Act
        await _sut.CreateAsync(prompt, cancellationToken);

        // Assert
        await _mongoCollection.Received(1).InsertOneAsync(
            Arg.Is<PromptDal>(x =>
                x.PromptId == prompt.PromptId &&
                x.Key == prompt.Key &&
                x.Value == prompt.Value &&
                x.OwnerId == prompt.OwnerId),
            Arg.Any<InsertOneOptions>(),
            cancellationToken);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePrompt_WhenPromptExists()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var storedPromptDal = new PromptDal
        {
            PromptId = prompt.PromptId,
            Key = _fixture.Create<string>(),
            Value = _fixture.Create<string>(),
            OwnerId = prompt.OwnerId
        };

        _mongoCursor.MoveNextAsync(CancellationToken.None).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns([storedPromptDal]);

        // Act
        await _sut.UpdateAsync(prompt, CancellationToken.None);

        // Assert
        await _mongoCollection.DidNotReceive().InsertOneAsync(Arg.Any<PromptDal>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
        await _mongoCollection.Received(1).ReplaceOneAsync(
                Arg.Any<FilterDefinition<PromptDal>>(),
                Arg.Is<PromptDal>(
                    x => x.PromptId == prompt.PromptId &&
                         x.Key == prompt.Key &&
                         x.Value == prompt.Value &&
                         x.OwnerId == prompt.OwnerId
                ),
                Arg.Is<ReplaceOptions>(x => x.IsUpsert == false),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePrompt()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var promptDal = prompt.ToDal();
        var cancellationToken = CancellationToken.None;

        _mongoCursor.MoveNextAsync(cancellationToken).ReturnsForAnyArgs(true);
        _mongoCursor.Current.Returns([promptDal]);

        _mongoCollection
            .DeleteOneAsync(Arg.Any<FilterDefinition<PromptDal>>(), cancellationToken)
            .Returns(new DeleteResult.Acknowledged(1));

        // Act
        var act = () => _sut.DeleteAsync(prompt, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
