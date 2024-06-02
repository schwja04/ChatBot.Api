using System.Collections.ObjectModel;

namespace ChatBot.Api.Application.Models.Queries;

public record GetChatHistoryMetadatasQueryResponse
{
    public required ReadOnlyCollection<ChatHistoryMetadata> ChatHistoryMetadatas { get; init; }
}
