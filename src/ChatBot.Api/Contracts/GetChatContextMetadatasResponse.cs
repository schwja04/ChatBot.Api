namespace ChatBot.Api.Contracts;

public record GetChatContextMetadatasResponse
{
    public required IReadOnlyCollection<GetChatContextMetadataResponse> ChatHistoryMetadatas { get; init; }
}
