using System.Collections.ObjectModel;

namespace ChatBot.Domain.PromptEntity;

public interface IPromptRepository : IReadPromptRepository, IWritePromptRepository
{
}

public interface IReadPromptRepository
{
    Task<Prompt?> GetAsync(Guid promptId, CancellationToken cancellationToken);
    Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken);
    Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken);
}

public interface IWritePromptRepository
{
    Task DeleteAsync(Prompt prompt, CancellationToken cancellationToken);
    Task CreateAsync(Prompt prompt, CancellationToken cancellationToken);
    Task UpdateAsync(Prompt prompt, CancellationToken cancellationToken);
}