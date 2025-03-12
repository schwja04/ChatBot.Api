using ChatBot.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Application.Commands.CreatePrompt;

public record CreatePromptCommand : IRequest<Prompt>
{
    public required string Key { get; init; }

    public required string Value { get; init; }

    public required Guid OwnerId { get; init; }
}
