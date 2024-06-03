namespace ChatBot.Api.Contracts;

public record ProcessChatMessageTitleRequest
{
    public required Guid ContextId { get; init; }

    public required string Title { get; init; }
}

