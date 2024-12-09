using System.Collections.ObjectModel;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Infrastructure.Repositories.Persistence.InMemory;

internal class PromptInMemoryRepository : IPromptRepository
{
    private readonly List<Prompt> _prompts = new()
    {
        Prompt.CreateNew(
            key: PromptKey.Email,
            value: PromptValue.Email,
            owner: "System"),
        Prompt.CreateNew(
            key: PromptKey.None,
            value: PromptValue.None,
            owner: "System"),
        Prompt.CreateNew(
            key: PromptKey.Title,
            value: PromptValue.Title,
            owner: "System"),
        Prompt.CreateNew(
            key: "Custom",
            value: "Does not matter {0}",
            owner: "Unknown"),
        Prompt.CreateNew(
            key: "ShouldNotShow",
            value: "Also does not matter {0}",
            owner: "schwjac"),
    };

    public Task<Prompt?> GetAsync(Guid promptId, CancellationToken cancellationToken)
    {
        var prompt = _prompts.FirstOrDefault(p => p.PromptId == promptId);
        
        if (prompt is null)
        {
            return Task.FromResult<Prompt?>(null);
        }
        
        // Make a copy of the prompt to avoid changing the original
        var copiedPrompt = prompt with { };
        return Task.FromResult<Prompt?>(copiedPrompt);
    }

    public Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken)
    {
        var userPrompt = _prompts.FirstOrDefault(prompt =>
            string.Equals(prompt.Owner, username, StringComparison.OrdinalIgnoreCase)
            && string.Equals(prompt.Key, promptKey, StringComparison.OrdinalIgnoreCase));

        if (userPrompt is null)
        {
            return Task.FromResult<Prompt?>(null);
        }
        
        // Make a copy of the prompt to avoid changing the original
        var copiedPrompt = userPrompt with { };
        
        return Task.FromResult<Prompt?>(copiedPrompt);
    }

    public Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken)
    {
        var userPrompts = _prompts
            .Where(prompt => string.Equals(prompt.Owner, username, StringComparison.OrdinalIgnoreCase))
            .Select(p => p with { })
            .ToList()
            .AsReadOnly();

        return Task.FromResult(userPrompts);
    }

    public Task CreateAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        // Make a copy of the prompt to avoid changing the original
        var copiedPrompt = prompt with { };
        _prompts.Add(copiedPrompt);
        
        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        var persistedPrompt = _prompts.Single(p => p.PromptId == prompt.PromptId);
        _prompts.Remove(persistedPrompt);

        return Task.CompletedTask;
    }
    
    public Task UpdateAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        var savedPrompt = _prompts.Single(p => p.PromptId == prompt.PromptId);
        _prompts.Remove(savedPrompt);
        
        // Make a copy of the prompt to avoid changing the original
        var copiedPrompt = prompt with { };
        _prompts.Add(copiedPrompt);

        return Task.CompletedTask;
    }
}

