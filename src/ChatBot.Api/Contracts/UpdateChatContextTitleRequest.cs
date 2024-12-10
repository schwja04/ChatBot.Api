namespace ChatBot.Api.Contracts;

public record UpdateChatContextTitleRequest
{
    public required string Title { get; init; }
}

