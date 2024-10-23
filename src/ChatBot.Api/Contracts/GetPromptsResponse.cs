using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Contracts;

public record GetPromptsResponse
{
    public required IReadOnlyCollection<Prompt> Prompts { get; init; }
}

