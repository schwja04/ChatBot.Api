using System.Net.Mime;
using ChatBot.Api.Authentication;
using ChatBot.Api.Contracts;
using ChatBot.Api.Mappers;
using ChatBot.Application.Commands.DeleteChatContext;
using ChatBot.Application.Commands.ProcessChatMessage;
using ChatBot.Application.Commands.UpdateChatContextTitle;
using ChatBot.Application.Queries.GetChatContext;
using ChatBot.Application.Queries.GetManyChatContextMetadata;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.Controllers;

[ApiController]
public class ChatsController(ILogger<ChatsController> logger, IMediator mediator) : ControllerBase
{
    private readonly ILogger _logger = logger;
    private readonly IMediator _mediator = mediator;

    private const string DefaultUsername = "Unknown";

    [HttpPost(Routes.Chats)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProcessChatMessageResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromBody] ProcessChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        var command = new ProcessChatMessageCommand
        {
            ContextId = request.ContextId,
            Content = request.Content,
            PromptKey = request.PromptKey,
            UserId = User.GetUserId()
        };
        
        var username = User.GetUsername();
        
        _logger.LogInformation(
            "Processing chat message for user {Username} ({UserId}) in context {ContextId}.",
            username,
            command.UserId,
            command.ContextId);
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(new ProcessChatMessageResponse
        {
            ContextId = response.ContextId,
            ChatMessage = response.ChatMessage.ToChatMessageResponse()
        });
    }

    [HttpGet(Routes.ChatMetadata)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetManyChatContextMetadataResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManyChatContextMetadataAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetManyChatContextMetadataQuery()
        {
            UserId = User.GetUserId()
        };
        
        var username = User.GetUsername();

        _logger.LogInformation("Getting chat context metadata records for {Username} ({UserId}).", username, query.UserId);
        var chatContextMetadatas = await _mediator.Send(query, cancellationToken);

        return Ok(new GetManyChatContextMetadataResponse
        {
            ChatHistoryMetadatas = chatContextMetadatas.Select(x => new GetChatContextMetadataResponse
            {
                ContextId = x.ContextId,
                Title = x.Title,
                UserId = x.UserId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToArray()
        });
    }

    [HttpGet(Routes.ChatsByContextId)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetChatContextResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromRoute] Guid contextId, CancellationToken cancellationToken = default)
    {
        var query = new GetChatContextQuery
        {
            ContextId = contextId,
            UserId = User.GetUserId()
        };

        var username = User.GetUsername();
        
        _logger.LogInformation("Getting chat context {ContextId} for {Username} ({UserId}).", query.ContextId, username, query.UserId);
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(new GetChatContextResponse
        {
            ContextId = response.ContextId,
            Title = response.Title,
            UserId = response.UserId,
            ChatMessages = response.Messages.ToChatMessageResponses(),
            CreatedAt = response.CreatedAt,
            UpdatedAt = response.UpdatedAt
        });
    }

    [HttpPut(Routes.UpdateChatTitleByContextId)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTitleAsync(
        [FromRoute] Guid contextId,
        [FromBody] UpdateChatContextTitleRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateChatContextTitleCommand
        {
            ContextId = contextId,
            Title = request.Title,
            UserId = User.GetUserId()
        };

        var username = User.GetUsername();
        
        _logger.LogInformation(
            "Updating chat context title for user {Username} ({UserId}) in context {ContextId}.",
            username,
            command.UserId,
            command.ContextId);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete(Routes.ChatsByContextId)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid contextId, CancellationToken cancellationToken = default)
    {
        var command = new DeleteChatContextCommand
        {
            ContextId = contextId,
            UserId = User.GetUserId()
        };
        
        var username = User.GetUsername();

        _logger.LogInformation(
            "Deleting chat context {ContextId} for user {Username} ({UserId}).",
            command.ContextId,
            username,
            command.UserId);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
