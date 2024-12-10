namespace ChatBot.Api.Contracts;

public record GetManyChatContextMetadataResponse
{
    public required IReadOnlyCollection<GetChatContextMetadataResponse> ChatHistoryMetadatas { get; init; }
}
