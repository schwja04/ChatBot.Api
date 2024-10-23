using System.Collections.ObjectModel;
using ChatBot.Api.Domain.Exceptions;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class PromptInMemoryRepository : IPromptRepository, IReadPromptRepository, IWritePromptRepository
{
    private readonly List<Prompt> _prompts;

    public PromptInMemoryRepository()
    {
        _prompts = new List<Prompt>()
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
    }

    public Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken)
    {
        var userPrompt = _prompts.FirstOrDefault(prompt =>
            string.Equals(prompt.Owner, username, StringComparison.OrdinalIgnoreCase)
            && string.Equals(prompt.Key, promptKey, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(userPrompt);
    }

    public Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken)
    {
        var userPrompts = _prompts
            .Where(prompt => string.Equals(prompt.Owner, username, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

        return Task.FromResult(userPrompts);
    }

    public Task DeleteAsync(string username, Guid promptId, CancellationToken cancellationToken)
    {
        Prompt? prompt = _prompts.FirstOrDefault(p => p.PromptId == promptId);

        if (prompt is null)
        {
            return Task.CompletedTask;
        }

        if (!string.Equals(prompt.Owner, username, StringComparison.OrdinalIgnoreCase))
        {
            throw new PromptAuthorizationException(promptId, username, prompt);
        }

        _prompts.Remove(prompt);

        return Task.CompletedTask;
    }

    public Task SaveAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        Prompt? savedPrompt = _prompts.FirstOrDefault(p => p.PromptId == prompt.PromptId);

        if (savedPrompt is null)
        {
            _prompts.Add(prompt);
            return Task.CompletedTask;
        }

        if (!string.Equals(prompt.Owner, savedPrompt.Owner, StringComparison.OrdinalIgnoreCase))
        {
            throw new PromptAuthorizationException(prompt.PromptId, prompt.Owner);

        }
        _prompts.Remove(savedPrompt);
        _prompts.Add(prompt);

        return Task.CompletedTask;
    }

    public Task<Prompt?> GetAsync(string username, Guid promptId, CancellationToken cancellationToken)
    {
        Prompt? prompt = _prompts.FirstOrDefault(p => p.PromptId == promptId);

        if (prompt is null)
        {
            return Task.FromResult<Prompt?>(null);
        }

        if (!string.Equals(prompt.Owner, username, StringComparison.OrdinalIgnoreCase))
        {
            throw new PromptAuthorizationException(promptId, username, prompt);
        }

        return Task.FromResult<Prompt?>(prompt);
    }
}

