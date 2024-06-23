using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Application.Models.Exceptions;
using ChatBot.Api.Infrastructure.MongoModels;
using ChatBot.Api.Infrastructure.Repositories.Mappers;
using Common.Mongo;
using MongoDB.Driver;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class PromptMongoRepository : IPromptRepository, IReadPromptRepository, IWritePromptRepository
{
    private readonly IMongoClientFactory _mongoClientFactory;

    public PromptMongoRepository(IMongoClientFactory mongoClientFactory)
    {
        _mongoClientFactory = mongoClientFactory;
    }

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
            await collection.InsertOneAsync(promptToSave, (InsertOneOptions?)null, cancellationToken);
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
            return;
        }

        if (!string.Equals(prompt.Owner, username, StringComparison.OrdinalIgnoreCase))
        {
            throw new PromptAuthorizationException(promptId, username, prompt);
        }

        var deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);

        if (deleteResult.DeletedCount == 0)
        {
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
