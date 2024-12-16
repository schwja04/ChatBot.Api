using AutoFixture;
using ChatBot.Api.Application.Commands.UpdateChatContextTitle;
using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Domain.Exceptions.ChatContextExceptions;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ChatBot.Api.UnitTests.Application.Handlers.CommandHandlers;

public class UpdateChatContextTitleCommandHandlerShould
{
    private readonly IChatContextRepository _chatContextRepository;
    private readonly UpdateChatContextTitleCommandHandler _sut;

    private readonly Fixture _fixture;

    public UpdateChatContextTitleCommandHandlerShould()
    {
        _chatContextRepository = Substitute.For<IChatContextRepository>();
        _sut = new UpdateChatContextTitleCommandHandler(_chatContextRepository);
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task Handle_WithValidContextId_ShouldUpdateTitle()
    {
        // Arrange
        var chatContext = ChatContext.CreateNew(username: _fixture.Create<string>());
        
        var cmd = _fixture.Build<UpdateChatContextTitleCommand>()
            .With(x => x.ContextId, chatContext.ContextId)
            .With(x => x.Username, chatContext.Username)
            .Create();
        
        _chatContextRepository.GetAsync(cmd.ContextId, Arg.Any<CancellationToken>()).Returns(chatContext);
        
        // Act
        await _sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        await _chatContextRepository.Received(1).SaveAsync(chatContext, Arg.Any<CancellationToken>());
        chatContext.Title.Should().Be(cmd.Title);
    }
    
    [Fact]
    public async Task Handle_WithInvalidContextId_ShouldThrowChatContextNotFoundException()
    {
        // Arrange
        var cmd = _fixture.Create<UpdateChatContextTitleCommand>();
        
        _chatContextRepository.GetAsync(cmd.ContextId, Arg.Any<CancellationToken>()).ReturnsNull();
        
        // Act
        Func<Task> act = async () => await _sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<ChatContextNotFoundException>();
    }
    
    [Fact]
    public async Task Handle_WithInvalidUsername_ShouldThrowChatContextAuthorizationException()
    {
        // Arrange
        var chatContext = ChatContext.CreateNew(username: _fixture.Create<string>());
        
        var cmd = _fixture.Build<UpdateChatContextTitleCommand>()
            .With(x => x.ContextId, chatContext.ContextId)
            .With(x => x.Username, _fixture.Create<string>())
            .Create();
        
        _chatContextRepository.GetAsync(cmd.ContextId, Arg.Any<CancellationToken>()).Returns(chatContext);
        
        // Act
        Func<Task> act = async () => await _sut.Handle(cmd, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<ChatContextAuthorizationException>();
    }
}