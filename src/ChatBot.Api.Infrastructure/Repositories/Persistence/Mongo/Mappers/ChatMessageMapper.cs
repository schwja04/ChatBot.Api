using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Domain.PromptEntity;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Models;

namespace ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Mappers;

internal static class ChatMessageMapper
{
    public static ChatMessageDal ToDal(this ChatMessage message)
    {
        return new ChatMessageDal
        {
            MessageId = message.MessageId,
            Content = message.Content,
            PromptKey = message.PromptKey,
            CreatedAt = message.CreatedAt,
            ChatterRole = Enum.GetName(message.Role)!.ToLowerInvariant()
        };
    }

    public static ChatMessage ToDomain(this ChatMessageDal message)
    {
        Enum.TryParse<ChatterRole>(message.ChatterRole, ignoreCase: true, out var chatterRole);

        return ChatMessage.CreateExistingChatMessage(
            messageId: message.MessageId,
            content: message.Content,
            promptKey: message.PromptKey ?? PromptKey.None,
            createdAt: message.CreatedAt,
            role: chatterRole);
    }
}
