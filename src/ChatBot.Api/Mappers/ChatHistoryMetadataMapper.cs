using System.Collections.ObjectModel;
using ChatBot.Api.Contracts;
using ChatBot.Api.Domain.ChatHistoryEntity;

namespace ChatBot.Api.Mappers;

public static class ChatHistoryMetadataMapper
{
    public static GetChatHistoryMetadataResponse ToResponse(this ChatHistoryMetadata metadata)
    {
        return new GetChatHistoryMetadataResponse
        {
            ContextId = metadata.ContextId,
            Title = metadata.Title,
            Username = metadata.Username,
            CreatedAt = metadata.CreatedAt,
            UpdatedAt = metadata.UpdatedAt
        };
    }

    public static ReadOnlyCollection<GetChatHistoryMetadataResponse> ToResponses(
        this IEnumerable<ChatHistoryMetadata> metadatas)
    {
        if (metadatas is null)
        {
            throw new ArgumentNullException(nameof(metadatas));
        }

        return metadatas.Select(ToResponse).ToList().AsReadOnly();
    }
}

