using Microsoft.Extensions.Options;

using ChatBot.Api.Mongo.Models;

namespace ChatBot.Api.Mongo;

public class MongoConfigManager : IMongoConfigManager
{
    private MongoConfigurationRecord _mongoConfigurationRecord;

    public MongoConfigManager(IOptionsMonitor<MongoConfigurationRecord> mongoConfigurationOption)
    {
        _mongoConfigurationRecord = mongoConfigurationOption.CurrentValue;

        mongoConfigurationOption.OnChange(OnConfigurationChange);
    }

    public MongoConfigManager(MongoConfigurationRecord mongoConfigurationRecord)
    {
        _mongoConfigurationRecord = mongoConfigurationRecord;
    }

    public MongoConfigurationRecord GetMongoConfigurationRecord()
    {
        return _mongoConfigurationRecord;
    }

    private void OnConfigurationChange(MongoConfigurationRecord mongoConfigurationRecord)
    {
        _mongoConfigurationRecord = mongoConfigurationRecord;
    }
}
