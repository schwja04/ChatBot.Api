using MongoDB.Driver;
using Common.Mongo.Models;

namespace Common.Mongo;

public interface IMongoClientFactory
{
    IMongoClient CreateClient();

    MongoConfigurationRecord GetMongoConfigurationRecord();
}
