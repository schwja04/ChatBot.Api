namespace ChatBot.Api.Contracts;

public record GetChatHistoryResponse
{
    public required Guid ContextId { get; init; }

    public required ChatMessageResponse[] ChatMessages { get; init; }
}
