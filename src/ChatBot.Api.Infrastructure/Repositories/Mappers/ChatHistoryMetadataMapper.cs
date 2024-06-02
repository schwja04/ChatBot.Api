using ChatBot.Api.Application.Models;
using ChatBot.Api.Infrastructure.MongoModels;

namespace ChatBot.Api.Infrastructure.Repositories.Mappers;

internal static class ChatHistoryMetadataMapper
{
    public static ChatHistoryMetadata ToDomain(this ChatHistoryMetadataDal metadata)
    {
        return ChatHistoryMetadata.CreateExisting(
            contextId: metadata.ContextId,
            title: metadata.Title,
            username: metadata.Username,
            createdAt: metadata.CreatedAt,
            updatedAt: metadata.UpdatedAt);
    }
}
