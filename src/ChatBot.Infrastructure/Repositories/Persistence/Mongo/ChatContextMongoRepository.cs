using System.Collections.ObjectModel;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo.Mappers;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo.Models;
using Common.Mongo;
using MongoDB.Driver;

namespace ChatBot.Infrastructure.Repositories.Persistence.Mongo;

internal class ChatContextMongoRepository(IMongoClientFactory mongoClientFactory) : IChatContextRepository
{
    private readonly IMongoClientFactory _mongoClientFactory = mongoClientFactory;
    
    public async Task SaveAsync(ChatContext chatContext, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        
        await collection.ReplaceOneAsync(
            Builders<ChatHistoryDal>.Filter.Eq(x => x.ContextId, chatContext.ContextId),
            chatContext.ToDal(),
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task<ChatContext?> GetAsync(Guid contextId, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<ChatHistoryDal>
            .Filter
            .Where(x => x.ContextId == contextId);

        using IAsyncCursor<ChatHistoryDal> result = await collection
            .FindAsync(filter, cancellationToken: cancellationToken);

        var chatHistoryDal = await result.FirstOrDefaultAsync(cancellationToken);

        return chatHistoryDal?.ToDomain();
    }

    public async Task<ReadOnlyCollection<ChatContextMetadata>> GetManyMetadataAsync(string username, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<ChatHistoryDal>
            .Filter
            .Where(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));

        using IAsyncCursor<ChatHistoryMetadataDal> result = await collection
            .FindAsync(filter, new FindOptions<ChatHistoryDal, ChatHistoryMetadataDal>()
            {
                Projection = Builders<ChatHistoryDal>
                    .Projection
                    .Expression(chatHistory => new ChatHistoryMetadataDal()
                    {
                        ContextId = chatHistory.ContextId,
                        Title = chatHistory.Title,
                        Username = chatHistory.Username,
                        CreatedAt = chatHistory.CreatedAt,
                        UpdatedAt = chatHistory.UpdatedAt,
                    })
            }, cancellationToken: cancellationToken);

        var chatHistoryMetadataDals = await result.ToListAsync(cancellationToken);

        return chatHistoryMetadataDals.Select(dal => dal.ToDomain()).ToList().AsReadOnly();
    }

    public async Task DeleteAsync(Guid contextId, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<ChatHistoryDal>
            .Filter
            .Where(x => x.ContextId == contextId);

        await collection.DeleteOneAsync(filter, options: null, cancellationToken);
    }

    private IMongoCollection<ChatHistoryDal> GetCollection()
    {
        return _mongoClientFactory
            .CreateClient()
            .GetDatabase(_mongoClientFactory.GetMongoConfigurationRecord().DatabaseName)
            .GetCollection<ChatHistoryDal>("ChatHistory");
    }
}

