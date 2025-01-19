using ChatBot.Domain.ChatContextEntity;

namespace ChatBot.Application.Abstractions.Repositories;

public interface IChatCompletionRepository
{
    Task<ChatMessage> GetChatCompletionAsync(ChatContext chatContext, CancellationToken cancellationToken);
}
