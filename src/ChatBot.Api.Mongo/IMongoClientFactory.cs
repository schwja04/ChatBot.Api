using MongoDB.Driver;

using ChatBot.Api.Mongo.Models;

namespace ChatBot.Api.Mongo;

public interface IMongoClientFactory
{
    IMongoClient CreateClient();

    MongoConfigurationRecord GetMongoConfigurationRecord();
}
