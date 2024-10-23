using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Commands;

public record CreatePromptCommand : IRequest<Prompt>
{
    public required string Key { get; init; }

    public required string Value { get; init; }

    public required string Owner { get; init; }
}

public record CreatePromptCommandResponse
{
    public required Prompt Prompt { get; init; } 
}
