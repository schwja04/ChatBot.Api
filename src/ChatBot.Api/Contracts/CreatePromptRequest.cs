namespace ChatBot.Api.Contracts;

public record CreatePromptRequest
{
    public required string Key { get; init; }

    public required string Value { get; init; }
}

public record CreatePromptResponse
{
    public required Guid PromptId { get; init; }

    public required string Key { get; init; }

    public required string Value { get; init; }

    public required string Owner { get; init; }
}
