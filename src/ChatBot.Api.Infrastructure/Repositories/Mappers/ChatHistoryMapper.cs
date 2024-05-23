using ChatBot.Api.Application.Models;
using ChatBot.Api.Infrastructure.MongoModels;

namespace ChatBot.Api.Infrastructure.Repositories.Mappers;

internal static class ChatHistoryMapper
{
    public static ChatHistoryDal ToDal(this ChatHistory history)
    {
        return new ChatHistoryDal
        {
            ContextId = history.ContextId,
            ChatMessages = history.ChatMessages.Select(message => message.ToDal()),
            CreatedAt = history.CreatedAt,
            UpdatedAt = history.UpdatedAt
        };
    }

    public static ChatHistory ToDomain(this ChatHistoryDal history)
    {
        return ChatHistory.CreateExisting(
            contextId: history.ContextId,
            messages: history.ChatMessages.Select(message => message.ToDomain()),
            createdAt: history.CreatedAt,
            updatedAt: history.UpdatedAt);
    }
}
