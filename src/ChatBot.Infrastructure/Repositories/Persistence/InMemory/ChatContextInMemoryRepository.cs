using System.Collections.ObjectModel;
using ChatBot.Domain.ChatContextEntity;

namespace ChatBot.Infrastructure.Repositories.Persistence.InMemory;

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

    public Task<ReadOnlyCollection<ChatContextMetadata>> GetManyMetadataAsync(Guid userId, CancellationToken cancellationToken)
    {
        var chatHistoryMetadatas = _chatHistories.Values
            .Where(chatHistory => chatHistory.UserId == userId)
            .Select(chatHistory => ChatContextMetadata.CreateExisting(
                contextId: chatHistory.ContextId,
                title: chatHistory.Title,
                userId: chatHistory.UserId,
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
