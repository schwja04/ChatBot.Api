using System.Collections.ObjectModel;
using ChatBot.Domain.PromptEntity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ChatBot.Infrastructure.Repositories.Persistence.Cached;

internal class CachedPromptRepository(
    ILogger<CachedPromptRepository> logger,
    IMemoryCache memoryCache,
    IPromptRepository promptRepository)
    : IPromptRepository
{
    private readonly ILogger _logger = logger;
    private readonly IMemoryCache _cache = memoryCache;
    private readonly IPromptRepository _promptRepository = promptRepository;

    public async Task<Prompt?> GetAsync(Guid promptId, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(promptId, out Prompt? prompt))
        {
            _logger.LogInformation(
                "Retrieved prompt ({PromptId}) from cache.",
                promptId);
            return prompt;
        }
        
        _logger.LogInformation(
            "Prompt ({PromptId}) not found in cache. Retrieving from repository.",
            promptId);
        prompt = await _promptRepository.GetAsync(promptId, cancellationToken);
        if (prompt is null)
        {
            _logger.LogInformation(
                "Prompt ({PromptId}) not found in repository.",
                promptId);
            return null;
        }
        
        await RefreshCacheAsync(prompt.Owner, cancellationToken);
        
        return prompt;
    }

    public async Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken)
    {
        var userCacheKey = CacheKeys.Single(username, promptKey);

        // First Check Cache
        if (_cache.TryGetValue(userCacheKey, out Prompt? prompt))
        {
            return prompt;
        }

        prompt = await _promptRepository.GetAsync(username, promptKey, cancellationToken);
        if (prompt is null)
        {
            return null;
        }

        await RefreshCacheAsync(username, cancellationToken);

        return prompt;
    }

    public async Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken)
    {
        var userAllCacheKey = CacheKeys.All(username);
        if (_cache.TryGetValue(userAllCacheKey, out ReadOnlyCollection<Prompt>? userPrompts))
        {
            _logger.LogInformation(
                "Retrieved prompts for user {Username} from cache.",
                username);
            return userPrompts!;
        }
        
        _logger.LogInformation(
            "Prompts for user {Username} not found in cache. Retrieving from repository.",
            username);
        userPrompts = await _promptRepository.GetManyAsync(username, cancellationToken);
        
        _logger.LogInformation(
            "Caching prompts for user {Username}.",
            username);
        foreach (var prompt in userPrompts)
        {
            var singleCacheKey = CacheKeys.Single(prompt.Owner, prompt.Key);
            _cache.Set(singleCacheKey, prompt, TimeSpan.FromDays(1));
            
            var singleCacheId = CacheKeys.Single(prompt.PromptId);
            _cache.Set(singleCacheId, prompt, TimeSpan.FromDays(1));
        }
        _cache.Set(userAllCacheKey, userPrompts, TimeSpan.FromDays(1));

        return userPrompts.ToList().AsReadOnly();
    }
    
    public async Task CreateAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        await _promptRepository.CreateAsync(prompt, cancellationToken);
        await RefreshCacheAsync(prompt.Owner, cancellationToken);
    }
    
    public async Task UpdateAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        await _promptRepository.UpdateAsync(prompt, cancellationToken);
        await RefreshCacheAsync(prompt.Owner, cancellationToken);
    }

    public async Task DeleteAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        await _promptRepository.DeleteAsync(prompt, cancellationToken);
        await RefreshCacheAsync(prompt.Owner, cancellationToken);
    }

    private async Task RefreshCacheAsync(string username, CancellationToken cancellationToken)
    {
        InvalidateCache(username);
        _ = await GetManyAsync(username, cancellationToken);
    }
    
    private void InvalidateCache(string username)
    {
        _logger.LogInformation(
            "Invalidating cache for user {Username}",
            username);
        var allCacheKey = CacheKeys.All(username);
        if (!_cache.TryGetValue(allCacheKey, out ReadOnlyCollection<Prompt>? value))
        {
            return;
        }

        foreach (var prompt in value!)
        {
            var singleCacheKey = CacheKeys.Single(prompt.Owner, prompt.Key);
            _cache.Remove(singleCacheKey);

            var singleCacheId = CacheKeys.Single(prompt.PromptId);
            _cache.Remove(singleCacheId);
        }

        _cache.Remove(allCacheKey);
    }
    
    private static class CacheKeys
    {
        public static string All(string username) => $"Prompts_{username}";
        public static string Single(string username, string promptKey) => $"Prompt_{username}_{promptKey}";
        public static string Single(Guid promptId) => $"Prompt_{promptId}";
    }
}

