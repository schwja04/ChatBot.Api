using ChatBot.Api.Domain.ChatHistoryEntity;

namespace ChatBot.Api.Application.Abstractions.Repositories;

public interface IChatCompletionRepository
{
    Task<ChatMessage> GetChatCompletionAsync(ChatHistory chatHistory, CancellationToken cancellationToken);
}
