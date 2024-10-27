using System.Collections.ObjectModel;
using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class ChatContextInMemoryRepository : IChatContextRepository
{
    private readonly Dictionary<Guid, ChatContext> _chatHistories = new();

    public Task SaveAsync(ChatContext chatContext, CancellationToken cancellationToken)
    {
        _chatHistories.Add(chatContext.ContextId, chatContext);
        return Task.CompletedTask;
    }

    public Task<ChatContext?> GetAsync(Guid contextId, CancellationToken cancellationToken)
    {
        if (!_chatHistories.TryGetValue(contextId, out ChatContext? chatHistory))
        {
            return Task.FromResult((ChatContext?)null);
        }

        return Task.FromResult<ChatContext?>(chatHistory);
    }

    public Task<ReadOnlyCollection<ChatContextMetadata>> GetManyMetadataAsync(string username, CancellationToken cancellationToken)
    {
        var chatHistoryMetadatas = _chatHistories.Values
            .Where(chatHistory => string.Equals(chatHistory.Username, username, StringComparison.OrdinalIgnoreCase))
            .Select(chatHistory => ChatContextMetadata.CreateExisting(
                contextId: chatHistory.ContextId,
                title: chatHistory.Title,
                username: chatHistory.Username,
                createdAt: chatHistory.CreatedAt,
                updatedAt: chatHistory.UpdatedAt))
            .ToList()
            .AsReadOnly();

        return Task.FromResult(chatHistoryMetadatas);
    }

    public Task DeleteAsync(Guid contextId, CancellationToken cancellationToken)
    {
        _chatHistories.Remove(contextId);

        return Task.CompletedTask;
    }
}