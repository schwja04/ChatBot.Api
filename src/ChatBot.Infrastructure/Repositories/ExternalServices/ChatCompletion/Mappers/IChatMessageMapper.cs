using System.Collections.ObjectModel;
using ChatBot.Domain.ChatContextEntity;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;

internal interface IChatMessageMapper
{
    Task<ReadOnlyCollection<ChatMessage>> ToLLMChatMessagesAsync(
        ChatContext chatContext,
        CancellationToken cancellationToken);
}