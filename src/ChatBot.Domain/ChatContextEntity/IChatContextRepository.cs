using System.Collections.ObjectModel;

namespace ChatBot.Api.Domain.ChatContextEntity;

public interface IChatContextRepository
{
    Task<ChatContext?> GetAsync(Guid contextId, CancellationToken cancellationToken);

    Task<ReadOnlyCollection<ChatContextMetadata>> GetManyMetadataAsync(string username, CancellationToken cancellationToken);

    Task SaveAsync(ChatContext chatContext, CancellationToken cancellationToken);

    Task DeleteAsync(Guid contextId, CancellationToken cancellationToken);
}
