using AutoFixture;
using ChatBot.Application.Abstractions.Repositories;
using ChatBot.Application.Commands.ProcessChatMessage;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.Exceptions.ChatContextExceptions;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ChatBot.UnitTests.Application.Handlers.CommandHandlers;

public class ProcessChatMessageCommandHandlerShould
{
    private readonly IChatCompletionRepository _chatCompletionRepository;
    private readonly IChatContextRepository _chatContextRepository;
    private readonly ProcessChatMessageCommandHandler _sut;
    
    private readonly Fixture _fixture;
    
    public ProcessChatMessageCommandHandlerShould()
    {
        _chatCompletionRepository = Substitute.For<IChatCompletionRepository>();
        _chatContextRepository = Substitute.For<IChatContextRepository>();
        _sut = new ProcessChatMessageCommandHandler(_chatCompletionRepository, _chatContextRepository);
        
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnResponse()
    {
        // Arrange
        var chatContext = ChatContext.CreateExisting(
            contextId: _fixture.Create<Guid>(),
            title: _fixture.Create<string>(),
            username: _fixture.Create<string>(),
            messages: new List<ChatMessage>(),
            createdAt: _fixture.Create<DateTimeOffset>(),
            updatedAt: _fixture.Create<DateTimeOffset>());
        var request = _fixture
            .Build<ProcessChatMessageCommand>()
            .With(x => x.ContextId, chatContext.ContextId)
            .With(x => x.Username, chatContext.Username)
            .Create();
        var cancellationToken = CancellationToken.None;
        
        var chatMessage = _fixture.Create<ChatMessage>();
        
        _chatContextRepository.GetAsync(request.ContextId, cancellationToken).Returns(chatContext);
        _chatCompletionRepository.GetChatCompletionAsync(chatContext, cancellationToken).Returns(chatMessage);
        
        // Act
        var result = await _sut.Handle(request, cancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.ContextId.Should().Be(chatContext.ContextId);
        result.ChatMessage.Should().Be(chatMessage);
    }
    
    [Fact]
    public async Task Handle_WithNewContext_ShouldSetTitle()
    {
        // Arrange
        var request = _fixture
            .Build<ProcessChatMessageCommand>()
            .With(x => x.ContextId, Guid.Empty)
            .Create();
        var cancellationToken = CancellationToken.None;
        
        var titleChatMessage = _fixture.Create<ChatMessage>();
        
        _chatContextRepository.GetAsync(request.ContextId, cancellationToken).ReturnsNull();
        _chatCompletionRepository.GetChatCompletionAsync(Arg.Any<ChatContext>(), cancellationToken).Returns(titleChatMessage);
        
        // Act
        await _sut.Handle(request, cancellationToken);
        
        // Assert
        
        await _chatContextRepository.Received(1).SaveAsync(
            Arg.Is<ChatContext>(x => !string.IsNullOrWhiteSpace(x.Title)),
            cancellationToken);
    }
    
    [Fact]
    public async Task Handle_WithInvalidContextId_ShouldThrowChatContextNotFoundException()
    {
        // Arrange
        var request = _fixture.Create<ProcessChatMessageCommand>();
        var cancellationToken = CancellationToken.None;
        
        _chatContextRepository.GetAsync(request.ContextId, cancellationToken).ReturnsNull();
        
        // Act
        Func<Task> act = async () => await _sut.Handle(request, cancellationToken);
        
        // Assert
        await act.Should().ThrowAsync<ChatContextNotFoundException>();
    }
    
    [Fact]
    public async Task Handle_WithInvalidUsername_ShouldThrowChatContextAuthorizationException()
    {
        // Arrange
        var chatContext = ChatContext.CreateExisting(
            contextId: _fixture.Create<Guid>(),
            title: _fixture.Create<string>(),
            username: _fixture.Create<string>(),
            messages: new List<ChatMessage>(),
            createdAt: _fixture.Create<DateTimeOffset>(),
            updatedAt: _fixture.Create<DateTimeOffset>());
        var request = _fixture
            .Build<ProcessChatMessageCommand>()
            .With(x => x.ContextId, chatContext.ContextId)
            .With(x => x.Username, _fixture.Create<string>())
            .Create();
        var cancellationToken = CancellationToken.None;
        
        _chatContextRepository.GetAsync(request.ContextId, cancellationToken).Returns(chatContext);
        
        // Act
        Func<Task> act = async () => await _sut.Handle(request, cancellationToken);
        
        // Assert
        await act.Should().ThrowAsync<ChatContextAuthorizationException>();
    }
}