using ChatBot.Api.Infrastructure.Repositories;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.UnitTests.Infrastructure.Repositories;

public class CachedUserAccessiblePromptRepositoryShould
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPromptRepository _innerPromptRepository;

    private readonly CachedUserAccessiblePromptRepository _sut;
    private readonly IFixture _fixture;

    public CachedUserAccessiblePromptRepositoryShould()
    {
        _memoryCache = Substitute.For<IMemoryCache>();
        _innerPromptRepository = Substitute.For<IPromptRepository>();

        _sut = new CachedUserAccessiblePromptRepository(_memoryCache, _innerPromptRepository);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnPromptFromSystemCache()
    {
        // Arrange
        var prompt = Prompt.CreateExisting(
            Guid.NewGuid(),
            "System",
            _fixture.Create<string>(),
            _fixture.Create<string>());

        var cancellationToken = CancellationToken.None;

        _memoryCache.TryGetValue(Arg.Any<object>(), out Arg.Any<object?>()).Returns(x =>
        {
            x[1] = prompt;
            return true;
        });

        // Act
        var result = await _sut.GetAsync("AnotherUser", prompt.PromptId, cancellationToken);

        // Assert
        result.Should().Be(prompt);
        _memoryCache.Received(1).TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());

        await _innerPromptRepository
            .DidNotReceive()
            .GetAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAsync_ShouldReturnPromptFromUserCache()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();

        var cancellationToken = CancellationToken.None;

        _memoryCache.TryGetValue(Arg.Any<object>(), out Arg.Any<object?>()).Returns(
        x => false,
        x =>
        {
            x[1] = prompt;
            return true;
        });

        // Act
        var result = await _sut.GetAsync(prompt.Owner, prompt.PromptId, cancellationToken);

        // Assert
        result.Should().Be(prompt);
        _memoryCache.Received(2).TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());

        await _innerPromptRepository
            .DidNotReceive()
            .GetAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAsync_ShouldReturnPromptFromSystemCacheAfterCacheRefresh()
    {
        // Arrange
        var prompt = Prompt.CreateExisting(
            Guid.NewGuid(),
            "System",
            _fixture.Create<string>(),
            _fixture.Create<string>());

        var cancellationToken = CancellationToken.None;

        string systemCacheId = string.Format("Prompt_{0}_{1}", prompt.Owner, prompt.PromptId);

        _memoryCache.TryGetValue(systemCacheId, out Arg.Any<object?>()).Returns(
        x => false,
        x =>
        {
            x[1] = prompt;
            return true;
        });

        _innerPromptRepository
            .GetManyAsync("System", Arg.Any<CancellationToken>())
            .Returns((new List<Prompt>(1) { prompt }).AsReadOnly());
        _innerPromptRepository
            .GetManyAsync(prompt.Owner, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Prompt>().AsReadOnly());

        // Act
        var result = await _sut.GetAsync(prompt.Owner, prompt.PromptId, cancellationToken);

        // Assert
        result.Should().Be(prompt);
        _memoryCache.Received(7).TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());

        await _innerPromptRepository
            .Received(2)
            .GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAsync_ShouldReturnPromptFromUserCacheAfterCacheRefresh()
    {
        // Arrange
        var prompt = Prompt.CreateExisting(
            Guid.NewGuid(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>());

        var cancellationToken = CancellationToken.None;

        string systemCacheId = string.Format("Prompt_{0}_{1}", prompt.Owner, prompt.PromptId);

        _memoryCache.TryGetValue(systemCacheId, out Arg.Any<object?>()).Returns(
        x => false,
        x =>
        {
            x[1] = prompt;
            return true;
        });

        _innerPromptRepository
            .GetManyAsync("System", Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Prompt>().AsReadOnly());
        _innerPromptRepository
            .GetManyAsync(prompt.Owner, Arg.Any<CancellationToken>())
            .Returns((new List<Prompt>(1) { prompt }).AsReadOnly());

        // Act
        var result = await _sut.GetAsync(prompt.Owner, prompt.PromptId, cancellationToken);

        // Assert
        result.Should().Be(prompt);
        _memoryCache.Received(7).TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());

        await _innerPromptRepository
            .Received(2)
            .GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNullAfterCacheMiss()
    {
        // Arrange
        var prompt = Prompt.CreateExisting(
            Guid.NewGuid(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>());

        var cancellationToken = CancellationToken.None;

        _innerPromptRepository
            .GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Prompt>().AsReadOnly());

        // Act
        var result = await _sut.GetAsync(prompt.Owner, prompt.PromptId, cancellationToken);

        // Assert
        result.Should().BeNull();
        _memoryCache.Received(7).TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());

        await _innerPromptRepository
            .Received(2)
            .GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetManyAsync_ShouldMissCache()
    {
        // Arrange
        string username = "System";
        var prompts = (new List<Prompt>(1)
        {
            Prompt.CreateNew(
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                username)
        }).AsReadOnly();

        var cancellationToken = CancellationToken.None;

        _innerPromptRepository.GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Prompt>().AsReadOnly());
        _innerPromptRepository.GetManyAsync(username, Arg.Any<CancellationToken>())
            .Returns(prompts);

        // Act
        var results = await _sut.GetManyAsync(username, cancellationToken);

        // Assert
        results.First().Should().Be(prompts.First());

        _memoryCache.Received(2).TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());
        _memoryCache.Received(6).CreateEntry(Arg.Any<object>());
    }

    [Fact]
    public async Task SaveAsync_ShouldRefreshCache()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var cancellationToken = CancellationToken.None;

        _innerPromptRepository
            .GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Prompt>().AsReadOnly());

        // Act
        await _sut.SaveAsync(prompt, cancellationToken);

        // Assert
        await _innerPromptRepository
            .Received(1)
            .SaveAsync(prompt, cancellationToken);
        await _innerPromptRepository
            .Received(2)
            .GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        _memoryCache
            .Received(3)
            .TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldRefreshCache()
    {
        // Arrange
        string username = _fixture.Create<string>();
        Guid promptId = _fixture.Create<Guid>();
        var cancellationToken = CancellationToken.None;

        _innerPromptRepository
            .GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Prompt>().AsReadOnly());

        // Act
        await _sut.DeleteAsync(username, promptId, cancellationToken);

        // Assert
        await _innerPromptRepository
            .Received(1)
            .DeleteAsync(username, promptId, cancellationToken);
        await _innerPromptRepository
            .Received(2)
            .GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        _memoryCache
            .Received(3)
            .TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());
    }
}
