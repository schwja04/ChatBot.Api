using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Contracts;

public record CreatePromptRequest
{
    public required string Key { get; init; }

    public required string Value { get; init; }
}

public record CreatePromptResponse
{
    public required Prompt Prompt { get; init; }
}
