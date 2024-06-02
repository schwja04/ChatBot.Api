using MongoDB.Driver;
using System.Collections.ObjectModel;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Infrastructure.MongoModels;
using ChatBot.Api.Infrastructure.Repositories.Mappers;
using Common.Mongo;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class ChatHistoryMongoRepository : IChatHistoryRepository
{
    private readonly IMongoClientFactory _mongoClientFactory;

    public ChatHistoryMongoRepository(IMongoClientFactory mongoClientFactory)
    {
        _mongoClientFactory = mongoClientFactory;
    }

    public Task SaveChatHistoryAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        var collection = GetCollection();

        var filter = Builders<ChatHistoryDal>
            .Filter
            .Where(x => x.ContextId == chatHistory.ContextId);

        return collection.ReplaceOneAsync(
            Builders<ChatHistoryDal>.Filter.Eq(x => x.ContextId, chatHistory.ContextId),
            chatHistory.ToDal(),
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task<ChatHistory?> GetChatHistoryAsync(Guid contextId, CancellationToken cancellationToken)
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

    public async Task<ReadOnlyCollection<ChatHistoryMetadata>> GetChatHistoryMetadatasAsync(string username, CancellationToken cancellationToken)
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

    private IMongoCollection<ChatHistoryDal> GetCollection()
    {
        return _mongoClientFactory
            .CreateClient()
            .GetDatabase(_mongoClientFactory.GetMongoConfigurationRecord().DatabaseName)
            .GetCollection<ChatHistoryDal>("ChatHistory");
    }
}

