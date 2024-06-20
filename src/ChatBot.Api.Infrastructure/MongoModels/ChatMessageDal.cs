using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatBot.Api.Infrastructure.MongoModels;

internal record ChatMessageDal
{
    [BsonRepresentation(BsonType.String)]
    public required Guid MessageId { get; init; }

    public required string Content { get; init; }

    public required string PromptKey { get; init; }

    [BsonRepresentation(BsonType.DateTime)]
    public required DateTimeOffset CreatedAt { get; init; }

    public required string ChatterRole { get; init; }
}
