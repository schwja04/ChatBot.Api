using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class ChatHistoryInMemoryRepository : IChatHistoryRepository
{
    private readonly Dictionary<Guid, ChatHistory> _chatHistories = new();

    public Task SaveChatHistoryAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        _chatHistories.Add(chatHistory.ContextId, chatHistory);
        return Task.CompletedTask;
    }

    public Task<ChatHistory?> GetChatHistoryAsync(Guid chatHistoryId, CancellationToken cancellationToken)
    {
        if (_chatHistories.TryGetValue(chatHistoryId, out ChatHistory? chatHistory))
        {
            return Task.FromResult<ChatHistory?>(chatHistory);
        }

        return Task.FromResult((ChatHistory?)null);
    }
}
