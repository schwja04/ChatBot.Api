using ChatBot.Api.Application.Models;

namespace ChatBot.Api.Application.Abstractions.Repositories;

public interface IChatCompletionRepository
{
    Task<ChatMessage> GetChatCompletionAsync(ChatHistory chatHistory, CancellationToken cancellationToken);
}
