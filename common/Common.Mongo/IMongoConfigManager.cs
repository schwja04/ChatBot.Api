using Common.Mongo.Models;

namespace Common.Mongo;

public interface IMongoConfigManager
{
    MongoConfigurationRecord GetMongoConfigurationRecord();
}
