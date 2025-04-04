using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatBot.Infrastructure.Repositories.Persistence.Mongo.Models;

internal record ChatHistoryDal
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required Guid ContextId { get; init; }

    public required string Title { get; init; }

    [BsonRepresentation(BsonType.String)]
    public required Guid UserId { get; init; }

    public required IEnumerable<ChatMessageDal> ChatMessages { get; init; }

    [BsonRepresentation(BsonType.DateTime)]
    public required DateTimeOffset CreatedAt { get; init; }

    [BsonRepresentation(BsonType.DateTime)]
    public required DateTimeOffset UpdatedAt { get; init; }
}
