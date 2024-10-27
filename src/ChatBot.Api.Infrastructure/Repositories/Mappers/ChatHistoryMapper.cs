using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Infrastructure.MongoModels;

namespace ChatBot.Api.Infrastructure.Repositories.Mappers;

internal static class ChatHistoryMapper
{
    public static ChatHistoryDal ToDal(this ChatContext context)
    {
        return new ChatHistoryDal
        {
            ContextId = context.ContextId,
            Title = context.Title,
            Username = context.Username,
            ChatMessages = context.ChatMessages.Select(message => message.ToDal()),
            CreatedAt = context.CreatedAt,
            UpdatedAt = context.UpdatedAt
        };
    }

    public static ChatContext ToDomain(this ChatHistoryDal history)
    {
        return ChatContext.CreateExisting(
            contextId: history.ContextId,
            title: history.Title,
            username: history.Username,
            messages: history.ChatMessages.Select(message => message.ToDomain()),
            createdAt: history.CreatedAt,
            updatedAt: history.UpdatedAt);
    }
}
