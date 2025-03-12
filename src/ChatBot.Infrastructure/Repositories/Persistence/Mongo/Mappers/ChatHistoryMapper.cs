using ChatBot.Domain.ChatContextEntity;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo.Models;

namespace ChatBot.Infrastructure.Repositories.Persistence.Mongo.Mappers;

internal static class ChatHistoryMapper
{
    public static ChatHistoryDal ToDal(this ChatContext context)
    {
        return new ChatHistoryDal
        {
            ContextId = context.ContextId,
            Title = context.Title,
            UserId = context.UserId,
            ChatMessages = context.Messages.Select(message => message.ToDal()),
            CreatedAt = context.CreatedAt,
            UpdatedAt = context.UpdatedAt
        };
    }

    public static ChatContext ToDomain(this ChatHistoryDal history)
    {
        return ChatContext.CreateExisting(
            contextId: history.ContextId,
            title: history.Title,
            userId: history.UserId,
            messages: history.ChatMessages.Select(message => message.ToDomain()),
            createdAt: history.CreatedAt,
            updatedAt: history.UpdatedAt);
    }
}
