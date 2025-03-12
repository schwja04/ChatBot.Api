using System.Collections.ObjectModel;

namespace ChatBot.Domain.PromptEntity;

public interface IPromptRepository : IReadPromptRepository, IWritePromptRepository
{
}

public interface IReadPromptRepository
{
    Task<Prompt?> GetAsync(Guid promptId, CancellationToken cancellationToken);
    Task<Prompt?> GetAsync(Guid userId, string promptKey, CancellationToken cancellationToken);
    Task<ReadOnlyCollection<Prompt>> GetManyAsync(Guid userId, CancellationToken cancellationToken);
}

public interface IWritePromptRepository
{
    Task DeleteAsync(Prompt prompt, CancellationToken cancellationToken);
    Task CreateAsync(Prompt prompt, CancellationToken cancellationToken);
    Task UpdateAsync(Prompt prompt, CancellationToken cancellationToken);
}