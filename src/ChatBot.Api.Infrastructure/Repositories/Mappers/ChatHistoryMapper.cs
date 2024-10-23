using ChatBot.Api.Domain.ChatHistoryEntity;
using ChatBot.Api.Infrastructure.MongoModels;

namespace ChatBot.Api.Infrastructure.Repositories.Mappers;

internal static class ChatHistoryMapper
{
    public static ChatHistoryDal ToDal(this ChatHistory history)
    {
        return new ChatHistoryDal
        {
            ContextId = history.ContextId,
            Title = history.Title,
            Username = history.Username,
            ChatMessages = history.ChatMessages.Select(message => message.ToDal()),
            CreatedAt = history.CreatedAt,
            UpdatedAt = history.UpdatedAt
        };
    }

    public static ChatHistory ToDomain(this ChatHistoryDal history)
    {
        return ChatHistory.CreateExisting(
            contextId: history.ContextId,
            title: history.Title,
            username: history.Username,
            messages: history.ChatMessages.Select(message => message.ToDomain()),
            createdAt: history.CreatedAt,
            updatedAt: history.UpdatedAt);
    }
}
