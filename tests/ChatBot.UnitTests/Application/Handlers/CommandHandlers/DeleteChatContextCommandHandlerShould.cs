using ChatBot.Application.Commands.DeleteChatContext;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.Exceptions.ChatContextExceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChatBot.UnitTests.Application.Handlers.CommandHandlers;

public class DeleteChatContextCommandHandlerShould
{
    private readonly IChatContextRepository _chatContextRepository;
    private readonly DeleteChatContextCommandHandler _sut;

    public DeleteChatContextCommandHandlerShould()
    {
        _chatContextRepository = Substitute.For<IChatContextRepository>();
        var logger = NullLogger<DeleteChatContextCommandHandler>.Instance;
        _sut = new DeleteChatContextCommandHandler(logger, _chatContextRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldThrowChatContextNotFoundException_WhenChatContextNotFound()
    {
        // Arrange
        var command = new DeleteChatContextCommand()
        {
            ContextId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        
        // Act
        var act = async () => await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<ChatContextNotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ShouldThrowChatContextAuthorizationException_WhenUserIsNotAuthorized()
    {
        // Arrange
        var chatContext = ChatContext.CreateExisting(
            Guid.NewGuid(),
            "test",
            Guid.NewGuid(),
            [],
            DateTime.UtcNow,
            DateTime.UtcNow);
        _chatContextRepository.GetAsync(chatContext.ContextId, Arg.Any<CancellationToken>()).Returns(chatContext);
        
        var command = new DeleteChatContextCommand()
        {
            ContextId = chatContext.ContextId,
            UserId = Guid.NewGuid(),
        };
        
        // Act
        var act = async () => await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<ChatContextAuthorizationException>();
    }
    
    [Fact]
    public async Task Handle_ShouldDeleteChatContext_WhenChatContextExistsAndUserIsAuthorized()
    {
        // Arrange
        var chatContext = ChatContext.CreateExisting(
            Guid.NewGuid(),
            "test",
            Guid.NewGuid(),
            [],
            DateTime.UtcNow,
            DateTime.UtcNow);
        _chatContextRepository.GetAsync(chatContext.ContextId, Arg.Any<CancellationToken>()).Returns(chatContext);
        
        var command = new DeleteChatContextCommand()
        {
            ContextId = chatContext.ContextId,
            UserId = chatContext.UserId
        };
        
        // Act
        await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        await _chatContextRepository.Received(1).DeleteAsync(chatContext.ContextId, Arg.Any<CancellationToken>());
    }
}