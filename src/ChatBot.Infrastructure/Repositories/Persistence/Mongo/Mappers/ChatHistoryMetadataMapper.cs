using ChatBot.Domain.ChatContextEntity;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo.Models;

namespace ChatBot.Infrastructure.Repositories.Persistence.Mongo.Mappers;

internal static class ChatHistoryMetadataMapper
{
    public static ChatContextMetadata ToDomain(this ChatHistoryMetadataDal metadata)
    {
        return ChatContextMetadata.CreateExisting(
            contextId: metadata.ContextId,
            title: metadata.Title,
            username: metadata.Username,
            createdAt: metadata.CreatedAt,
            updatedAt: metadata.UpdatedAt);
    }
}
