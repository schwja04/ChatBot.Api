using System.Collections.ObjectModel;
using ChatBot.Domain.PromptEntity;

namespace ChatBot.Infrastructure.Repositories.Persistence.InMemory;

internal class PromptInMemoryRepository : IPromptRepository
{
    private readonly List<Prompt> _prompts = new()
    {
        Prompt.CreateNew(
            key: PromptKey.Email,
            value: PromptValue.Email,
            ownerId: Guid.Empty),
        Prompt.CreateNew(
            key: PromptKey.None,
            value: PromptValue.None,
            ownerId: Guid.Empty),
        Prompt.CreateNew(
            key: PromptKey.Title,
            value: PromptValue.Title,
            ownerId: Guid.Empty),
        Prompt.CreateNew(
            key: "Custom",
            value: "Does not matter {0}",
            ownerId: Guid.NewGuid()),
        Prompt.CreateNew(
            key: "ShouldNotShow",
            value: "Also does not matter {0}",
            ownerId: Guid.NewGuid()),
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

    public Task<Prompt?> GetAsync(Guid userId, string promptKey, CancellationToken cancellationToken)
    {
        var userPrompt = _prompts.FirstOrDefault(prompt =>
            prompt.OwnerId == userId
            && string.Equals(prompt.Key, promptKey, StringComparison.OrdinalIgnoreCase));

        if (userPrompt is null)
        {
            return Task.FromResult<Prompt?>(null);
        }
        
        // Make a copy of the prompt to avoid changing the original
        var copiedPrompt = userPrompt with { };
        
        return Task.FromResult<Prompt?>(copiedPrompt);
    }

    public Task<ReadOnlyCollection<Prompt>> GetManyAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userPrompts = _prompts
            .Where(prompt => prompt.OwnerId == userId)
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

