using System.Collections.ObjectModel;
using System.Net.Mime;
using ChatBot.Api.Authentication;
using ChatBot.Api.Contracts;
using ChatBot.Api.Mappers;
using ChatBot.Application.Commands.CreatePrompt;
using ChatBot.Application.Commands.DeletePrompt;
using ChatBot.Application.Commands.UpdatePrompt;
using ChatBot.Application.Queries.GetManyPrompts;
using ChatBot.Application.Queries.GetPrompt;
using ChatBot.Domain.PromptEntity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.Controllers;

[ApiController]
public class PromptsController(ILogger<PromptsController> logger, IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger _logger = logger;

    private static readonly string DefaultUsername = "Unknown";

    [HttpGet(Routes.Prompts)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetManyPromptsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManyAsync(
        [FromQuery] bool includeSystemPrompts = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetManyPromptsQuery()
        {
            UserId = User.GetUserId(),
            IncludeSystemPrompts = includeSystemPrompts
        };

        var username = User.GetUsername();

        _logger.LogInformation(
            "Getting prompts for user {Username} ({UserId}). Included system prompts: {IncludeSystemPrompts}",
            username,
            query.UserId,
            query.IncludeSystemPrompts);
        ReadOnlyCollection<Prompt> prompts = await _mediator.Send(query, cancellationToken);

        return Ok(new GetManyPromptsResponse
        {
            Prompts = prompts.Select(x => new GetPromptResponse()
            {
                PromptId = x.PromptId,
                Key = x.Key,
                Value = x.Value,
                OwnerId = x.OwnerId
            }).ToList().AsReadOnly()
        });
    }

    [HttpGet(Routes.PromptsByPromptId)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetPromptResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] Guid promptId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        string username = User.Identity?.Name ?? DefaultUsername;

        var command = new GetPromptQuery
        {
            PromptId = promptId,
            UserId = userId
        };

        _logger.LogInformation("Getting prompt {PromptId} for user {Username} ({UserId})", promptId, username, userId);
        var prompt = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Prompt {PromptId} found for user {Username} ({UserId})", promptId, username, userId);
        return Ok(prompt.ToGetPromptResponse());
    }

    [HttpPost(Routes.Prompts)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreatePromptResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreatePromptRequest request, CancellationToken cancellationToken = default)
    {
        var username = User.GetUsername();
        
        var command = new CreatePromptCommand
        {
            Key = request.Key,
            Value = request.Value,
            OwnerId = User.GetUserId()
        };

        _logger.LogInformation("Creating prompt for user {Username} ({UserId})", username, command.OwnerId);
        Prompt prompt = await _mediator.Send(command, cancellationToken);

        var uri = new Uri(
            Routes.PromptsByPromptId.Replace("{promptId}", prompt.PromptId.ToString()),
            UriKind.Relative);

        _logger.LogInformation("Prompt {PromptId} created for user {Username} ({UserId})", prompt.PromptId, username, command.OwnerId);
        return Created(uri, prompt.ToCreatePromptResponse());
    }

    [HttpDelete(Routes.PromptsByPromptId)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid promptId, CancellationToken cancellationToken = default)
    {
        var username = User.GetUsername();

        var command = new DeletePromptCommand
        {
            PromptId = promptId,
            UserId = User.GetUserId()
        };

        _logger.LogInformation("Deleting prompt {PromptId} for user {Username} ({UserId})", promptId, username, command.UserId);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPut(Routes.PromptsByPromptId)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid promptId, [FromBody] UpdatePromptRequest request, CancellationToken cancellationToken = default)
    {
        string username = User.GetUsername();

        var command = new UpdatePromptCommand
        {
            PromptId = promptId,
            Key = request.Key,
            Value = request.Value,
            UserId = User.GetUserId()
        };

        _logger.LogInformation("Updating prompt {PromptId} for user {Username} ({UserId})", promptId, username, command.UserId);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
