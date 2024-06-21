using System.Net.Mime;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Application.Models.Commands;
using ChatBot.Api.Application.Models.Queries;
using ChatBot.Api.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.Controllers;

[ApiController]
public class PromptsController : ControllerBase
{
    private readonly IMediator _mediator;

    private static readonly string DefaultUsername = "Unknown";

    public PromptsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(Routes.Prompts)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetPromptsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManyAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetPromptsQuery()
        {
            Username = User.Identity?.Name ?? DefaultUsername
        };

        var response = await _mediator.Send(query, cancellationToken);

        return Ok(new GetPromptsResponse
        {
            Prompts = response.Prompts,
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

        var response = await _mediator.Send(command, cancellationToken);

        if (response.Prompt is not null)
        {
            return Ok(response.Prompt!);
        }

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

        var response = await _mediator.Send(command, cancellationToken);

        var uri = new Uri(
            Routes.PromptsByPromptId.Replace("{promptId}", response.Prompt.PromptId.ToString()),
            UriKind.Relative);

        return Created(uri, new CreatePromptResponse { Prompt = response.Prompt });
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

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
