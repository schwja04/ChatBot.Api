using System.Collections.ObjectModel;

namespace ChatBot.Api.Domain.PromptEntity;

public interface IPromptRepository : IReadPromptRepository, IWritePromptRepository
{
}

public interface IReadPromptRepository
{
    Task<Prompt?> GetAsync(string username, Guid promptId, CancellationToken cancellationToken);
    Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken);
    Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken);
}

public interface IWritePromptRepository
{
    Task DeleteAsync(string username, Guid promptId, CancellationToken cancellationToken);
    Task SaveAsync(Prompt prompt, CancellationToken cancellationToken);
}