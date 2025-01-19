using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatBot.Infrastructure.Repositories.Persistence.Mongo.Models;

internal record PromptDal
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required Guid PromptId { get; init; }

    public required string Key { get; init; }

    public required string Value { get; init; }

    public required string Owner { get; init; }
}

