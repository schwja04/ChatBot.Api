using AutoFixture;
using ChatBot.Application.Queries.GetChatContext;
using ChatBot.Domain.ChatContextEntity;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChatBot.UnitTests.Application.Handlers.QueryHandlers;

public class GetChatContextQueryHandlerShould
{
    private readonly IChatContextRepository _chatContextRepository;
    private readonly GetChatContextQueryHandler _sut;
    
    private readonly Fixture _fixture;
    
    public GetChatContextQueryHandlerShould()
    {
        _chatContextRepository = Substitute.For<IChatContextRepository>();
        var logger = NullLogger<GetChatContextQueryHandler>.Instance;
        _sut = new GetChatContextQueryHandler(logger, _chatContextRepository);
        
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task HandleAsync_WithValidRequest_ShouldReturnChatContext()
    {
        // Arrange
        var chatContext = _fixture.Create<ChatContext>();
        var query = new GetChatContextQuery
        {
            ContextId = chatContext.ContextId,
            UserId = chatContext.UserId
        };

        _chatContextRepository.GetAsync(query.ContextId, Arg.Any<CancellationToken>()).Returns(chatContext);
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().Be(chatContext);
    }
}