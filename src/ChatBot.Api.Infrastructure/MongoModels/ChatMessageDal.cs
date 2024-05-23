using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatBot.Api.Infrastructure.MongoModels;

internal record ChatMessageDal
{
    [BsonRepresentation(BsonType.String)]
    public Guid MessageId { get; init; }

    public required string Content { get; init; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset CreatedAt { get; init; }

    public required string ChatterRole { get; init; }
}
