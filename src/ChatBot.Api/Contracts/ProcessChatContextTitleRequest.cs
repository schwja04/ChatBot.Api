namespace ChatBot.Api.Contracts;

public record ProcessChatContextTitleRequest
{
    public required Guid ContextId { get; init; }

    public required string Title { get; init; }
}

