using System.Collections.ObjectModel;
using ChatBot.Api.Application.Models;

namespace ChatBot.Api.Application.Abstractions.Repositories;

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