namespace ChatBot.Api.Contracts;

public record ProcessChatMessageResponse
{
    public required Guid ContextId { get; init; }

    public required ChatMessageResponse ChatMessage { get; init; }
}

