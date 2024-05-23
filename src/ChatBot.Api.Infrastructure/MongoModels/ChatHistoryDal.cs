using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatBot.Api.Infrastructure.MongoModels;

internal record ChatHistoryDal
{
    [BsonId]
    //[BsonGuidRepresentation(GuidRepresentation.Standard)]
    [BsonRepresentation(BsonType.String)]
    public Guid ContextId { get; init; }

    public required IEnumerable<ChatMessageDal> ChatMessages { get; init; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset CreatedAt { get; init; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset UpdatedAt { get; init; }
}
