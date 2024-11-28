using ChatBot.Api.Domain.Exceptions;
using ChatBot.Api.Domain.PromptEntity;
using ChatBot.Api.Infrastructure.MongoModels;
using ChatBot.Api.Infrastructure.Repositories.Mappers;
using Common.Mongo;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class PromptMongoRepository(
    ILogger<PromptMongoRepository> logger, 
    IMongoClientFactory mongoClientFactory)
    : IPromptRepository
{
    private readonly ILogger<PromptMongoRepository> _logger = logger;
    private readonly IMongoClientFactory _mongoClientFactory = mongoClientFactory;

    public async Task<Prompt?> GetAsync(string username, Guid promptId, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<PromptDal>
            .Filter
            .Where(x =>
                string.Equals(x.Owner, username, StringComparison.OrdinalIgnoreCase)
                && x.PromptId == promptId);

        using IAsyncCursor<PromptDal> result = await collection
            .FindAsync(filter, cancellationToken: cancellationToken);

        var promptDal = await result.FirstOrDefaultAsync(cancellationToken);

        return promptDal?.ToDomain();
    }
    public async Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<PromptDal>
            .Filter
            .Where(x =>
                string.Equals(x.Owner, username, StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.Key, promptKey, StringComparison.OrdinalIgnoreCase));

        using IAsyncCursor<PromptDal> result = await collection
            .FindAsync(filter, cancellationToken: cancellationToken);

        var promptDal = await result.FirstOrDefaultAsync(cancellationToken);

        return promptDal?.ToDomain();
    }

    public async Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<PromptDal>
            .Filter
            .Where(x =>
                string.Equals(x.Owner, "System", StringComparison.OrdinalIgnoreCase)
                || string.Equals(x.Owner, username, StringComparison.OrdinalIgnoreCase));

        using IAsyncCursor<PromptDal> result = await collection
            .FindAsync(filter, cancellationToken: cancellationToken);

        var promptDals = await result.ToListAsync(cancellationToken);

        return promptDals.Select(x => x.ToDomain()).ToList().AsReadOnly();
    }

    public async Task SaveAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var promptToSave = prompt.ToDal();

        var filter = Builders<PromptDal>
            .Filter
            .Where(x => x.PromptId == promptToSave.PromptId);

        using IAsyncCursor<PromptDal> result = await collection
            .FindAsync(filter, cancellationToken: cancellationToken);

        PromptDal? savedPrompt = await result.FirstOrDefaultAsync(cancellationToken);

        if (savedPrompt is null)
        {
            await collection.InsertOneAsync(promptToSave, options: null, cancellationToken);
            return;
        }

        if (!string.Equals(promptToSave.Owner, savedPrompt.Owner, StringComparison.OrdinalIgnoreCase))
        {
            throw new PromptAuthorizationException(prompt.PromptId, prompt.Owner);
        }

        await collection.ReplaceOneAsync(
            filter,
            promptToSave,
            new ReplaceOptions
            {
                IsUpsert = false,
            },
            cancellationToken);
    }

    public async Task DeleteAsync(string username, Guid promptId, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<PromptDal>
            .Filter
            .Where(x => x.PromptId == promptId);

        using IAsyncCursor<PromptDal> result = await collection
            .FindAsync(filter, cancellationToken: cancellationToken);

        PromptDal? prompt = await result.FirstOrDefaultAsync(cancellationToken);

        if (prompt is null)
        {
            _logger.LogInformation("Prompt {PromptId} not found while attempting to delete", promptId);
            return;
        }

        if (!string.Equals(prompt.Owner, username, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Prompt {PromptId} was not deleted as it is not owned by {Username}", promptId, username);
            throw new PromptAuthorizationException(promptId, username, prompt);
        }

        var deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);

        if (deleteResult.DeletedCount == 0)
        {
            _logger.LogWarning("Prompt {PromptId} was not deleted. Only possibility is that a different caller got there first.", promptId);
            throw new UnreachableException();
        }
    }

    private IMongoCollection<PromptDal> GetCollection()
    {
        IMongoClient client = _mongoClientFactory.CreateClient();

        IMongoDatabase database = client.GetDatabase(_mongoClientFactory.GetMongoConfigurationRecord().DatabaseName);

        return database.GetCollection<PromptDal>("Prompts");
    }
}
