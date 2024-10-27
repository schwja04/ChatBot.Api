using System.Collections.ObjectModel;
using ChatBot.Api.Contracts;
using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Mappers;

public static class ChatContextMetadataMapper
{
    public static GetChatContextMetadataResponse ToResponse(this ChatContextMetadata metadata)
    {
        return new GetChatContextMetadataResponse
        {
            ContextId = metadata.ContextId,
            Title = metadata.Title,
            Username = metadata.Username,
            CreatedAt = metadata.CreatedAt,
            UpdatedAt = metadata.UpdatedAt
        };
    }

    public static ReadOnlyCollection<GetChatContextMetadataResponse> ToResponses(
        this IEnumerable<ChatContextMetadata> metadatas)
    {
        if (metadatas is null)
        {
            throw new ArgumentNullException(nameof(metadatas));
        }

        return metadatas.Select(ToResponse).ToList().AsReadOnly();
    }
}

