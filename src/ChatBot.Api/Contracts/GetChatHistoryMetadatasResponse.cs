namespace ChatBot.Api.Contracts;

public record GetChatHistoryMetadatasResponse
{
    public required IReadOnlyCollection<GetChatHistoryMetadataResponse> ChatHistoryMetadatas { get; init; }
}
