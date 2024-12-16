using AutoFixture;
using ChatBot.Api.Application.Queries.GetChatContext;
using ChatBot.Api.Domain.ChatContextEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.Api.UnitTests.Application.Handlers.QueryHandlers;

public class GetChatContextQueryHandlerShould
{
    private readonly IChatContextRepository _chatContextRepository;
    private readonly GetChatContextQueryHandler _sut;
    
    private readonly Fixture _fixture;
    
    public GetChatContextQueryHandlerShould()
    {
        _chatContextRepository = Substitute.For<IChatContextRepository>();
        _sut = new GetChatContextQueryHandler(_chatContextRepository);
        
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
            Username = chatContext.Username
        };

        _chatContextRepository.GetAsync(query.ContextId, Arg.Any<CancellationToken>()).Returns(chatContext);
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().Be(chatContext);
    }
}