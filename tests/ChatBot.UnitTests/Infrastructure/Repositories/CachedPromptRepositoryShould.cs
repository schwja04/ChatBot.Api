using AutoFixture;
using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.Repositories.Persistence.Cached;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChatBot.UnitTests.Infrastructure.Repositories;

public class CachedPromptRepositoryShould
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPromptRepository _innerPromptRepository;

    private readonly CachedPromptRepository _sut;
    private readonly IFixture _fixture;

    public CachedPromptRepositoryShould()
    {
        _memoryCache = Substitute.For<IMemoryCache>();
        _innerPromptRepository = Substitute.For<IPromptRepository>();
        var logger = NullLogger<CachedPromptRepository>.Instance;

        _sut = new CachedPromptRepository(logger, _memoryCache, _innerPromptRepository);
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task GetAsync_WithPromptId_ShouldReturnPrompt_FromCache()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var cancellationToken = CancellationToken.None;

        _memoryCache.TryGetValue(Arg.Any<object>(), out Arg.Any<object?>()).Returns(x =>
        {
            x[1] = prompt;
            return true;
        });
        _innerPromptRepository.GetAsync(prompt!.PromptId, cancellationToken).Returns(prompt);

        // Act
        var result = await _sut.GetAsync(prompt.PromptId, cancellationToken);

        // Assert
        result.Should().Be(prompt);
    }
    
    [Fact]
    public async Task GetAsync_WithPromptId_ShouldNotRefreshCache_WhenPromptIsNull()
    {
        // Arrange
        var promptId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _innerPromptRepository.GetAsync(promptId, cancellationToken).Returns((Prompt?)null);

        // Act
        var result = await _sut.GetAsync(promptId, cancellationToken);

        // Assert
        result.Should().BeNull();
        await _innerPromptRepository
            .Received(1)
            .GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task GetAsync_WithPromptId_ShouldReturnPrompt_FromRepository_AndRefreshCache()
    {
        // Arrange
        var prompt = _fixture.Create<Prompt>();
        var promptCollection = new List<Prompt> { prompt }.AsReadOnly();
        var cancellationToken = CancellationToken.None;

        _memoryCache.TryGetValue(Arg.Any<object>(), out Arg.Any<object?>()).Returns(
            _ => false);
        _innerPromptRepository.GetAsync(prompt.PromptId, cancellationToken).Returns(prompt);
        _innerPromptRepository.GetManyAsync(prompt.Owner, cancellationToken).Returns(promptCollection);
        
        // Act
        var result = await _sut.GetAsync(prompt.PromptId, cancellationToken);

        // Assert
        result.Should().Be(prompt);
        await _innerPromptRepository
            .Received(1)
            .GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        
        _memoryCache
            .Received(3)
            .TryGetValue(Arg.Any<object>(), out Arg.Any<object?>());
    }
}
