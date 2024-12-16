using System.Collections.ObjectModel;
using System.Diagnostics;
using ChatBot.Api.Domain.PromptEntity;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Mappers;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Models;
using Common.Mongo;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo;

internal class PromptMongoRepository(
    ILogger<PromptMongoRepository> logger, 
    IMongoClientFactory mongoClientFactory)
    : IPromptRepository
{
    private readonly ILogger<PromptMongoRepository> _logger = logger;
    private readonly IMongoClientFactory _mongoClientFactory = mongoClientFactory;

    public async Task<Prompt?> GetAsync(Guid promptId, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<PromptDal>
            .Filter
            .Where(x => x.PromptId == promptId);

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
    
    public async Task CreateAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var promptToSave = prompt.ToDal();

        await collection.InsertOneAsync(promptToSave, options: null, cancellationToken);
    }
    
    public async Task DeleteAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<PromptDal>
            .Filter
            .Where(x => x.PromptId == prompt.PromptId);

        var deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);

        if (deleteResult.DeletedCount == 0)
        {
            _logger.LogWarning("Prompt {PromptId} was not deleted. Only possibility is that a different caller got there first.", prompt.PromptId);
            throw new UnreachableException();
        }
    }

    public async Task UpdateAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var promptToSave = prompt.ToDal();

        var filter = Builders<PromptDal>
            .Filter
            .Where(x => x.PromptId == promptToSave.PromptId);
        
        await collection.ReplaceOneAsync(
            filter,
            promptToSave,
            new ReplaceOptions
            {
                IsUpsert = false,
            },
            cancellationToken);
    }

    private IMongoCollection<PromptDal> GetCollection()
    {
        IMongoClient client = _mongoClientFactory.CreateClient();

        IMongoDatabase database = client.GetDatabase(_mongoClientFactory.GetMongoConfigurationRecord().DatabaseName);

        return database.GetCollection<PromptDal>("Prompts");
    }
}
