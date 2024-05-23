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
            Content = request.Content
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ProcessChatMessageResponse
        {
            ContextId = response.ContextId,
            ChatMessage = response.ChatMessage.ToChatMessageResponse()
        });
    }

    [HttpGet("{contextId}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetChatHistoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromRoute] Guid contextId, CancellationToken cancellationToken = default)
    {
        var query = new GetChatHistoryQuery
        {
            ContextId = contextId
        };

        var response = await _mediator.Send(query, cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(new GetChatHistoryResponse
        {
            ContextId = response.ChatHistory.ContextId,
            ChatMessages = response.ChatHistory.ChatMessages.ToChatMessageResponses()
        });
    }
}
