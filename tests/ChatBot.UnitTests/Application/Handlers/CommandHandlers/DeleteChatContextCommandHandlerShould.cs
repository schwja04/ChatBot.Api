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
            Username = "test"
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
            "testUser",
            [],
            DateTime.UtcNow,
            DateTime.UtcNow);
        _chatContextRepository.GetAsync(chatContext.ContextId, Arg.Any<CancellationToken>()).Returns(chatContext);
        
        var command = new DeleteChatContextCommand()
        {
            ContextId = chatContext.ContextId,
            Username = "testUser2"
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
            "testUser",
            [],
            DateTime.UtcNow,
            DateTime.UtcNow);
        _chatContextRepository.GetAsync(chatContext.ContextId, Arg.Any<CancellationToken>()).Returns(chatContext);
        
        var command = new DeleteChatContextCommand()
        {
            ContextId = chatContext.ContextId,
            Username = chatContext.Username
        };
        
        // Act
        await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        await _chatContextRepository.Received(1).DeleteAsync(chatContext.ContextId, Arg.Any<CancellationToken>());
    }
}