using Common.Mongo.Models;
using MongoDB.Driver;

namespace Common.Mongo;

public interface IMongoClientFactory
{
    IMongoClient CreateClient();

    MongoConfigurationRecord GetMongoConfigurationRecord();
}
