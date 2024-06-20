using System.Collections.ObjectModel;

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

    public Task<ChatHistory?> GetChatHistoryAsync(Guid contextId, CancellationToken cancellationToken)
    {
        if (!_chatHistories.TryGetValue(contextId, out ChatHistory? chatHistory))
        {
            return Task.FromResult((ChatHistory?)null);
        }

        return Task.FromResult<ChatHistory?>(chatHistory);
    }

    public Task<ReadOnlyCollection<ChatHistoryMetadata>> GetChatHistoryMetadatasAsync(string username, CancellationToken cancellationToken)
    {
        var chatHistoryMetadatas = _chatHistories.Values
            .Where(chatHistory => string.Equals(chatHistory.Username, username, StringComparison.OrdinalIgnoreCase))
            .Select(chatHistory => ChatHistoryMetadata.CreateExisting(
                contextId: chatHistory.ContextId,
                title: chatHistory.Title,
                username: chatHistory.Username,
                createdAt: chatHistory.CreatedAt,
                updatedAt: chatHistory.UpdatedAt))
            .ToList()
            .AsReadOnly();

        return Task.FromResult(chatHistoryMetadatas);
    }

    public Task DeleteChatHistoryAsync(Guid contextId, CancellationToken cancellationToken)
    {
        _chatHistories.Remove(contextId);

        return Task.CompletedTask;
    }
}
