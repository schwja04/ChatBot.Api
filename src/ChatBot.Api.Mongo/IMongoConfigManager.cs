using ChatBot.Api.Mongo.Models;

namespace ChatBot.Api.Mongo;

public interface IMongoConfigManager
{
    MongoConfigurationRecord GetMongoConfigurationRecord();
}
