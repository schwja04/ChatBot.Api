using AutoFixture;
using ChatBot.Api.Contracts;
using ChatBot.Api.Controllers;
using ChatBot.Application.Commands.ProcessChatMessage;
using ChatBot.Application.Queries.GetChatContext;
using ChatBot.Application.Queries.GetManyChatContextMetadata;
using ChatBot.Domain.ChatContextEntity;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChatBot.UnitTests.Api.Controllers;

public class ChatsControllerShould
{
    private readonly IMediator _mediator;
    private readonly ChatsController _sut;
    
    private readonly Fixture _fixture;

    public ChatsControllerShould()
    {
        _mediator = Substitute.For<IMediator>();
        ILogger<ChatsController> logger = NullLogger<ChatsController>.Instance;
        _sut = new ChatsController(logger, _mediator);
        
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task PostAsync_WithValidRequest_ShouldReturnChatMessage()
    {
        // Arrange
        var request = _fixture.Create<ProcessChatMessageRequest>();
        var cmdResponse = _fixture.Create<ProcessChatMessageCommandResponse>();
        var username = _fixture.Create<string>();
        
        _mediator.Send(Arg.Any<ProcessChatMessageCommand>(), Arg.Any<CancellationToken>())
            .Returns(cmdResponse);
        
        _sut.ControllerContext = TestHelpers.CreateTestControllerContext(username);
        
        // Act
        var result = await _sut.PostAsync(request, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeOfType<ProcessChatMessageResponse>();
    }
    
    [Fact]
    public async Task GetManyChatContextMetadataAsync_WithValidRequest_ShouldReturnChatContextMetadatas()
    {
        // Arrange
        var chatContextMetadatas = _fixture
            .CreateMany<ChatContextMetadata>(3)
            .ToList()
            .AsReadOnly();
        var username = _fixture.Create<string>();
        
        _mediator.Send(Arg.Any<GetManyChatContextMetadataQuery>(), Arg.Any<CancellationToken>())
            .Returns(chatContextMetadatas);
        
        _sut.ControllerContext = TestHelpers.CreateTestControllerContext(username);
        
        // Act
        var result = await _sut.GetManyChatContextMetadataAsync(CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeOfType<GetManyChatContextMetadataResponse>();
    }
    
    [Fact]
    public async Task GetAsync_WithExistingChatContext_ShouldReturnGetChatContextResponse()
    {
        // Arrange
        var chatContext = _fixture.Create<ChatContext>();
        var contextId = chatContext.ContextId;
        var username = chatContext.Username;
        
        _mediator.Send(Arg.Any<GetChatContextQuery>(), Arg.Any<CancellationToken>())
            .Returns(chatContext);
        
        _sut.ControllerContext = TestHelpers.CreateTestControllerContext(username);
        
        // Act
        var result = await _sut.GetAsync(contextId, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeOfType<GetChatContextResponse>();
    }
    
    [Fact]
    public async Task UpdateTitleAsync_WithValidRequest_ShouldReturnNoContent()
    {
        // Arrange
        var chatContext = _fixture.Create<ChatContext>();
        var contextId = chatContext.ContextId;
        var username = chatContext.Username;
        var request = _fixture.Create<UpdateChatContextTitleRequest>();
        
        _sut.ControllerContext = TestHelpers.CreateTestControllerContext(username);
        
        // Act
        var result = await _sut.UpdateTitleAsync(contextId, request, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
    
    [Fact]
    public async Task DeleteAsync_WithExistingChatContext_ShouldReturnNoContent()
    {
        // Arrange
        var chatContext = _fixture.Create<ChatContext>();
        var contextId = chatContext.ContextId;
        var username = chatContext.Username;
        
        _sut.ControllerContext = TestHelpers.CreateTestControllerContext(username);
        
        // Act
        var result = await _sut.DeleteAsync(contextId, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}