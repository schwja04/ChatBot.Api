using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Cryptography.X509Certificates;

using ChatBot.Api.Mongo.Models;

namespace ChatBot.Api.Mongo;

public class MongoClientFactory : IMongoClientFactory
{
    private readonly IX509Manager _x509Manager;

    private object _lock = new();
    private MongoConfigurationRecord _configuration;
    private X509Certificate2? _certificate;

    private IMongoClient _cachedMongoClient = null!;


    public MongoClientFactory(IOptionsMonitor<MongoConfigurationRecord> mongoConfigurationOption, IX509Manager x509Manager)
    {
        ArgumentNullException.ThrowIfNull(mongoConfigurationOption, nameof(mongoConfigurationOption));

        _x509Manager = x509Manager;
        _configuration = mongoConfigurationOption.CurrentValue;

        _certificate = x509Manager.GetCertificate();
        mongoConfigurationOption.OnChange(OnConfigurationChange);
    }

    public MongoClientFactory(MongoConfigurationRecord mongoConfigurationRecord, IX509Manager x509Manager)
    {
        ArgumentNullException.ThrowIfNull(mongoConfigurationRecord, nameof(mongoConfigurationRecord));

        _configuration = mongoConfigurationRecord;
        _x509Manager = x509Manager;

        _certificate = x509Manager.GetCertificate();
    }

    public IMongoClient CreateClient()
    {
        return _cachedMongoClient ??= LoadMongoClient();
    }

    public MongoConfigurationRecord GetMongoConfigurationRecord()
    {
        return _configuration;
    }

    private void OnConfigurationChange(MongoConfigurationRecord mongoConfigurationRecord)
    {
        _configuration = mongoConfigurationRecord;

        _cachedMongoClient = LoadMongoClient();
    }

    private IMongoClient LoadMongoClient()
    {
        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
        {
            lock (_lock)
            {
                var settings = MongoClientSettings.FromConnectionString(_configuration.ConnectionString);
                settings.Credential = MongoCredential.CreateCredential(_configuration.DatabaseName, _configuration.Username, _configuration.Password);

                _cachedMongoClient = new MongoClient(settings);

                return _cachedMongoClient;
            }
        }

        var certificate = _x509Manager.GetCertificate();

        if (_cachedMongoClient is not null && ReferenceEquals(certificate, _certificate))
        {
            return _cachedMongoClient;
        }

        lock (_lock)
        {
            var pkiSettings = MongoClientSettings.FromUrl(new MongoUrl(_configuration.ConnectionString));
            pkiSettings.Credential = MongoCredential.CreateMongoX509Credential(null);
            pkiSettings.UseTls = true;

            pkiSettings.SslSettings = new SslSettings
            {
                ClientCertificates = new[] { certificate }
            };

            _cachedMongoClient = new MongoClient(pkiSettings);

            _certificate = certificate;

            return _cachedMongoClient;
        }

    }
}
