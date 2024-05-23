using ChatBot.Api.Application.Models;

namespace ChatBot.Api.Application.Abstractions.Repositories;

public interface IChatHistoryRepository
{
    Task<ChatHistory?> GetChatHistoryAsync(Guid contextId, CancellationToken cancellationToken);

    Task SaveChatHistoryAsync(ChatHistory chatHistory, CancellationToken cancellationToken);
}
