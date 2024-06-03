using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatBot.Api.Application.Models.Commands;
using ChatBot.Api.Application.Models.Queries;
using ChatBot.Api.Contracts;
using ChatBot.Api.Mappers;
using System.Net.Mime;

namespace ChatBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IMediator _mediator;

    private static readonly string DefaultUsername = "Unknown";

    public ChatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProcessChatMessageResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromBody] ProcessChatMessageRequest request, CancellationToken cancellationToken = default)
    {

        var command = new ProcessChatMessageCommand
        {
            ContextId = request.ContextId,
            Content = request.Content,
            Username = User.Identity?.Name ?? DefaultUsername
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ProcessChatMessageResponse
        {
            ContextId = response.ContextId,
            ChatMessage = response.ChatMessage.ToChatMessageResponse()
        });
    }

    [HttpGet("Metadatas")]
    [Produces(MediaTypeNames.Application.Json)]
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

    [HttpGet("{contextId}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetChatHistoryResponse), StatusCodes.Status200OK)]
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

    [HttpPut("{contextId}:UpdateTitle")]
    public async Task<IActionResult> UpdateTitleAsync([FromRoute] Guid contextId, [FromBody] ProcessChatMessageTitleRequest request, CancellationToken cancellationToken = default)
    {
        var command = new ProcessChatMessageTitleCommand
        {
            ContextId = contextId,
            Title = request.Title,
            Username = User.Identity?.Name ?? DefaultUsername
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}

