using System.Collections.ObjectModel;

namespace ChatBot.Domain.ChatContextEntity;

public interface IChatContextRepository
{
    Task<ChatContext?> GetAsync(Guid contextId, CancellationToken cancellationToken);

    Task<ReadOnlyCollection<ChatContextMetadata>> GetManyMetadataAsync(Guid userId, CancellationToken cancellationToken);

    Task SaveAsync(ChatContext chatContext, CancellationToken cancellationToken);

    Task DeleteAsync(Guid contextId, CancellationToken cancellationToken);
}
