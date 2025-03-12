using MediatR;

namespace ChatBot.Application.Commands.UpdatePrompt;

public record UpdatePromptCommand : IRequest
{
    public required Guid PromptId { get; init; }

    public required string Key { get; init; }

    public required string Value { get; init; }

    public required Guid UserId { get; init; }
}
