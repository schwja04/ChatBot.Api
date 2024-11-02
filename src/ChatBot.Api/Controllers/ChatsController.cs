using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Contracts;
using ChatBot.Api.Domain.Exceptions;
using ChatBot.Api.Mappers;

namespace ChatBot.Api.Controllers;

[ApiController]
public class ChatsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    private static readonly string DefaultUsername = "Unknown";
    
    [HttpPost(Routes.Chats)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProcessChatMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostAsync([FromBody] ProcessChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        var command = new ProcessChatMessageCommand
        {
            ContextId = request.ContextId,
            Content = request.Content,
            PromptKey = request.PromptKey,
            Username = User.Identity?.Name ?? DefaultUsername
        };

        try
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ProcessChatMessageResponse
            {
                ContextId = response.ContextId,
                ChatMessage = response.ChatMessage.ToChatMessageResponse()
            });
        }
        catch (ChatContextNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet(Routes.ChatMetadatas)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetChatContextMetadatasResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManyChatContextMetadataAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetManyChatContextMetadataQuery()
        {
            UserName = User.Identity?.Name ?? DefaultUsername,
        };

        var response = await _mediator.Send(query, cancellationToken);

        return Ok(new GetChatContextMetadatasResponse
        {
            ChatHistoryMetadatas = response.ChatContextMetadatas.Select(x => new GetChatContextMetadataResponse
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] Guid contextId, CancellationToken cancellationToken = default)
    {
        var query = new GetChatContextQuery
        {
            ContextId = contextId,
            Username = User.Identity?.Name ?? DefaultUsername
        };

        var response = await _mediator.Send(query, cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(new GetChatContextResponse
        {
            ContextId = response.ChatContext.ContextId,
            Title = response.ChatContext.Title,
            Username = response.ChatContext.Username,
            ChatMessages = response.ChatContext.Messages.ToChatMessageResponses(),
            CreatedAt = response.ChatContext.CreatedAt,
            UpdatedAt = response.ChatContext.UpdatedAt
        });
    }

    [HttpPut(Routes.UpdateChatTitleByContextId)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTitleAsync(
        [FromRoute] Guid contextId,
        [FromBody] ProcessChatContextTitleRequest request,
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
