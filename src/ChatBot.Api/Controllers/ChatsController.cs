using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Contracts;
using ChatBot.Api.Mappers;

namespace ChatBot.Api.Controllers;

[ApiController]
public class ChatsController : ControllerBase
{
    private readonly IMediator _mediator;

    private static readonly string DefaultUsername = "Unknown";

    public ChatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

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

    [HttpGet(Routes.ChatMetadatas)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetChatHistoryMetadatasResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChatHistoryMetadataAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetChatHistoryMetadatasQuery()
        {
            UserName = User.Identity?.Name ?? DefaultUsername,
        };

        var response = await _mediator.Send(query, cancellationToken);

        return Ok(new GetChatHistoryMetadatasResponse
        {
            ChatHistoryMetadatas = response.ChatHistoryMetadatas.Select(x => new GetChatHistoryMetadataResponse
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
    [ProducesResponseType(typeof(GetChatHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] Guid contextId, CancellationToken cancellationToken = default)
    {
        var query = new GetChatHistoryQuery
        {
            ContextId = contextId,
            Username = User.Identity?.Name ?? DefaultUsername
        };

        var response = await _mediator.Send(query, cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(new GetChatHistoryResponse
        {
            ContextId = response.ChatHistory.ContextId,
            Title = response.ChatHistory.Title,
            Username = response.ChatHistory.Username,
            ChatMessages = response.ChatHistory.ChatMessages.ToChatMessageResponses(),
            CreatedAt = response.ChatHistory.CreatedAt,
            UpdatedAt = response.ChatHistory.UpdatedAt
        });
    }

    [HttpPut(Routes.UpdateChatTitleByContextId)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTitleAsync(
        [FromRoute] Guid contextId,
        [FromBody] ProcessChatMessageTitleRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateChatMessageTitleCommand
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
        var command = new DeleteChatHistoryCommand
        {
            ContextId = contextId,
            Username = User.Identity?.Name ?? DefaultUsername
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
