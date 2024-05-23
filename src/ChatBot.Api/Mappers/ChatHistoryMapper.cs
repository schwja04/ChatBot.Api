using ChatBot.Api.Application.Models;
using ChatBot.Api.Contracts;

namespace ChatBot.Api.Mappers;

internal static class ChatHistoryMapper
{
    public static ChatMessageResponse ToChatMessageResponse(this ChatMessage chatMessage)
    {
        if (chatMessage is null)
        {
            throw new ArgumentNullException(nameof(chatMessage));
        }

        return new ChatMessageResponse
        {
            MessageId = chatMessage.MessageId,
            Role = Enum.GetName(chatMessage.Role)!.ToLowerInvariant(),
            Content = chatMessage.Content,
            CreatedAt = chatMessage.CreatedAt
        };
    }

    public static ChatMessageResponse[] ToChatMessageResponses(this IEnumerable<ChatMessage> chatMessages)
    {
        if (chatMessages is null)
        {
            throw new ArgumentNullException(nameof(chatMessages));
        }

        return chatMessages.Select(ToChatMessageResponse).ToArray();
    }
}
