using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ChatBot.Api.Contracts;
using ChatBot.Api.Mappers;
using ChatBot.Application.Commands.DeleteChatContext;
using ChatBot.Application.Commands.ProcessChatMessage;
using ChatBot.Application.Commands.UpdateChatContextTitle;
using ChatBot.Application.Queries.GetChatContext;
using ChatBot.Application.Queries.GetManyChatContextMetadata;

namespace ChatBot.Api.Controllers;

[ApiController]
public class ChatsController(IMediator mediator) : ControllerBase
{
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
            Username = User.Identity?.Name ?? DefaultUsername
        };
        
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
            Username = User.Identity?.Name ?? DefaultUsername,
        };

        var chatContextMetadatas = await _mediator.Send(query, cancellationToken);

        return Ok(new GetManyChatContextMetadataResponse
        {
            ChatHistoryMetadatas = chatContextMetadatas.Select(x => new GetChatContextMetadataResponse
            {
                ContextId = x.ContextId,
                Title = x.Title,
                Username = x.Username,
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
            Username = User.Identity?.Name ?? DefaultUsername
        };

        var response = await _mediator.Send(query, cancellationToken);

        return Ok(new GetChatContextResponse
        {
            ContextId = response.ContextId,
            Title = response.Title,
            Username = response.Username,
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
            Username = User.Identity?.Name ?? DefaultUsername
        };

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
            Username = User.Identity?.Name ?? DefaultUsername
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
