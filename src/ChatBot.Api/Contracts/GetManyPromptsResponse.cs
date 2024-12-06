using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Contracts;

public record GetManyPromptsResponse
{
    public required IReadOnlyCollection<GetPromptResponse> Prompts { get; init; }
}

public record GetPromptResponse
{
    public required Guid PromptId { get; init; }
    public required string Key { get; init; }
    public required string Value { get; init; }
    public required string Owner { get; init; }
}

