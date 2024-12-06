using System.Collections.ObjectModel;
using System.Net.Mime;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Application.Commands.CreatePrompt;
using ChatBot.Api.Application.Commands.DeletePrompt;
using ChatBot.Api.Application.Commands.UpdatePrompt;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Application.Queries.GetManyPrompts;
using ChatBot.Api.Application.Queries.GetPrompt;
using ChatBot.Api.Contracts;
using ChatBot.Api.Domain.PromptEntity;
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
    public async Task<IActionResult> GetManyAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetManyPromptsQuery()
        {
            Username = User.Identity?.Name ?? DefaultUsername
        };

        _logger.LogInformation("Getting prompts for {Username}", query.Username);
        ReadOnlyCollection<Prompt> prompts = await _mediator.Send(query, cancellationToken);

        return Ok(new GetManyPromptsResponse
        {
            Prompts = prompts.Select(x => new GetPromptResponse()
            {
                PromptId = x.PromptId,
                Key = x.Key,
                Value = x.Value,
                Owner = x.Owner
            }).ToList().AsReadOnly()
        });
    }

    [HttpGet(Routes.PromptsByPromptId)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(Prompt), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] Guid promptId, CancellationToken cancellationToken = default)
    {
        string username = User.Identity?.Name ?? DefaultUsername;

        var command = new GetPromptQuery(username, promptId);

        _logger.LogInformation("Getting prompt {PromptId} for {Username}", promptId, username);
        Prompt? prompt = await _mediator.Send(command, cancellationToken);

        if (prompt is not null)
        {
            _logger.LogInformation("Prompt {PromptId} found for {Username}", promptId, username);
            return Ok(prompt!);
        }

        _logger.LogInformation("Prompt {PromptId} not found for {Username}", promptId, username);
        return NotFound();
    }

    [HttpPost(Routes.Prompts)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreatePromptResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreatePromptRequest request, CancellationToken cancellationToken = default)
    {
        string username = User.Identity?.Name ?? DefaultUsername;

        var command = new CreatePromptCommand
        {
            Key = request.Key,
            Value = request.Value,
            Owner = username
        };

        _logger.LogInformation("Creating prompt for {Username}", username);
        Prompt prompt = await _mediator.Send(command, cancellationToken);

        var uri = new Uri(
            Routes.PromptsByPromptId.Replace("{promptId}", prompt.PromptId.ToString()),
            UriKind.Relative);

        _logger.LogInformation("Prompt {PromptId} created for {Username}", prompt.PromptId, username);
        return Created(uri, new CreatePromptResponse { Prompt = prompt });
    }

    [HttpDelete(Routes.PromptsByPromptId)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid promptId, CancellationToken cancellationToken = default)
    {
        string username = User.Identity?.Name ?? DefaultUsername;

        var command = new DeletePromptCommand
        {
            PromptId = promptId,
            Username = username
        };

        _logger.LogInformation("Deleting prompt {PromptId} for {Username}", promptId, username);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPut(Routes.PromptsByPromptId)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid promptId, [FromBody] UpdatePromptRequest request, CancellationToken cancellationToken = default)
    {
        string username = User.Identity?.Name ?? DefaultUsername;

        var command = new UpdatePromptCommand
        {
            PromptId = promptId,
            Key = request.Key,
            Value = request.Value,
            Owner = username
        };

        _logger.LogInformation("Updating prompt {PromptId} for {Username}", promptId, username);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
