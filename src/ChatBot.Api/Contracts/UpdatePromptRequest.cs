namespace ChatBot.Api.Contracts;

public record UpdatePromptRequest
{
    public required string Key { get; init; }

    public required string Value { get; init; }
}
