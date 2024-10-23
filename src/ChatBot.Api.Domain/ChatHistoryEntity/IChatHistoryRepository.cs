using System.Collections.ObjectModel;

namespace ChatBot.Api.Domain.ChatHistoryEntity;

public interface IChatHistoryRepository
{
    Task<ChatHistory?> GetChatHistoryAsync(Guid contextId, CancellationToken cancellationToken);

    Task<ReadOnlyCollection<ChatHistoryMetadata>> GetChatHistoryMetadatasAsync(string username, CancellationToken cancellationToken);

    Task SaveChatHistoryAsync(ChatHistory chatHistory, CancellationToken cancellationToken);

    Task DeleteChatHistoryAsync(Guid contextId, CancellationToken cancellationToken);
}
