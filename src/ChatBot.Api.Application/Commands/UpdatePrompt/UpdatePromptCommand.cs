using MediatR;

namespace ChatBot.Api.Application.Commands.UpdatePrompt;

public record UpdatePromptCommand : IRequest
{
    public required Guid PromptId { get; init; }

    public required string Key { get; init; }

    public required string Value { get; init; }

    public required string Owner { get; init; }
}
