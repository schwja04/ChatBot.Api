using System.Collections.ObjectModel;
using ChatBot.Api.Domain.PromptEntity;
using Microsoft.Extensions.Caching.Memory;

namespace ChatBot.Api.Infrastructure.Repositories.Persistence.Cached;

internal class CachedUserAccessiblePromptRepository(
    IMemoryCache memoryCache,
    IPromptRepository promptRepository)
    : IPromptRepository
{
    private const string System = nameof(System);
    private static readonly string AllPromptsKey = "AllPrompts_{0}";
    private static readonly string SinglePromptKey = "Prompt_{0}_{1}";

    private readonly IMemoryCache _cache = memoryCache;
    private readonly IPromptRepository _promptRepository = promptRepository;

    public async Task<Prompt?> GetAsync(string username, Guid promptId, CancellationToken cancellationToken)
    {
        return await GetAsync(username, (object)promptId, cancellationToken);
    }

    public async Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken)
    {
        return await GetAsync(username, (object)promptKey, cancellationToken);
    }

    public async Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken)
    {
        var systemPrompts = await HelperAsync(_cache, _promptRepository, System, cancellationToken);
        var userPrompts = await HelperAsync(_cache, _promptRepository, username, cancellationToken);

        var returnPrompts = new List<Prompt>(systemPrompts.Count + userPrompts.Count);
        returnPrompts.AddRange(systemPrompts);
        returnPrompts.AddRange(userPrompts);

        return returnPrompts.Distinct().ToList().AsReadOnly();


        static async Task<ReadOnlyCollection<Prompt>> HelperAsync(IMemoryCache cache, IReadPromptRepository readPromptRepository, string innerUsername, CancellationToken innerCancellationToken)
        {
            string userAllCacheKey = string.Format(AllPromptsKey, innerUsername);
            bool userCacheHit = cache.TryGetValue(userAllCacheKey, out ReadOnlyCollection<Prompt>? innerUserPrompts);

            if (!userCacheHit)
            {
                innerUserPrompts = await readPromptRepository.GetManyAsync(innerUsername, innerCancellationToken);
                foreach (Prompt prompt in innerUserPrompts)
                {
                    string singleCacheKey = string.Format(SinglePromptKey, prompt.Owner, prompt.Key);
                    cache.Set(singleCacheKey, prompt, TimeSpan.FromDays(1));

                    string singleCacheId = string.Format(SinglePromptKey, prompt.Owner, prompt.PromptId);
                    cache.Set(singleCacheId, prompt, TimeSpan.FromDays(1));
                }
                cache.Set(userAllCacheKey, innerUserPrompts, TimeSpan.FromDays(1));
            }

            return innerUserPrompts!;
        }
    }

    public async Task SaveAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        await _promptRepository.SaveAsync(prompt, cancellationToken);

        InvalidateCache(prompt.Owner);

        await GetManyAsync(prompt.Owner, cancellationToken);
    }

    public async Task DeleteAsync(string username, Guid promptId, CancellationToken cancellationToken)
    {
        await _promptRepository.DeleteAsync(username, promptId, cancellationToken);

        InvalidateCache(username);

        await GetManyAsync(username, cancellationToken);
    }

    private async Task<Prompt?> GetAsync(string username, object promptIdentifier, CancellationToken cancellationToken)
    {
        string systemCacheKey = string.Format(SinglePromptKey, System, promptIdentifier);
        string userCacheKey = string.Format(SinglePromptKey, username, promptIdentifier);

        // First Check Cache
        if (TryGetValue(systemCacheKey, userCacheKey, out Prompt? value))
        {
            return value;
        }

        // Invalidate cache and rebuild cache
        InvalidateCache(username);
        await GetManyAsync(username, cancellationToken);

        // Double Check Cache
        if (TryGetValue(systemCacheKey, userCacheKey, out value))
        {
            return value;
        }

        // Give up, temporarily cache a null
        _cache.Set(userCacheKey, (Prompt?)null, TimeSpan.FromMinutes(60));

        return null;
    }

    private bool TryGetValue(string systemCacheKey, string userCacheKey, out Prompt? value)
    {
        if (_cache.TryGetValue(systemCacheKey, out value))
        {
            return true;
        }

        if (_cache.TryGetValue(userCacheKey, out value))
        {
            return true;
        }

        return false;
    }

    private void InvalidateCache(string username)
    {
        string allCacheKey = string.Format(AllPromptsKey, username);
        if (!_cache.TryGetValue(allCacheKey, out ReadOnlyCollection<Prompt>? value))
        {
            return;
        }

        foreach (var prompt in value!)
        {
            string singleCacheKey = string.Format(SinglePromptKey, prompt.Owner, prompt.Key);
            _cache.Remove(singleCacheKey);

            string singleCacheId = string.Format(SinglePromptKey, prompt.Owner, prompt.PromptId);
            _cache.Remove(singleCacheId);
        }

        _cache.Remove(allCacheKey);
    }
}

