using MongoDB.Driver;
using System.Security.Cryptography.X509Certificates;

using Common.Mongo.Models;

namespace Common.Mongo;

public class MongoClientFactory(IMongoConfigManager mongoConfigManger, IX509Manager x509Manager)
    : IMongoClientFactory
{
    private readonly IX509Manager _x509Manager = x509Manager;
    private readonly IMongoConfigManager _mongoConfigManager = mongoConfigManger;

    private object _lock = new();
    private MongoConfigurationRecord _configuration = mongoConfigManger.GetMongoConfigurationRecord();
    private X509Certificate2? _certificate = x509Manager.GetCertificate();

    private IMongoClient? _cachedMongoClient;


    public IMongoClient CreateClient()
    {
        var mongoConfigurationRecord = _mongoConfigManager.GetMongoConfigurationRecord();
        var certificate = _x509Manager.GetCertificate();

        if (_cachedMongoClient is not null
                && ReferenceEquals(mongoConfigurationRecord, _configuration)
                && ReferenceEquals(certificate, _certificate))
        {
            return _cachedMongoClient;
        }

        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
        {
            lock (_lock)
            {
                var settings = MongoClientSettings.FromConnectionString(_configuration.ConnectionString);
                // settings.Credential = MongoCredential.CreateCredential(
                //         _configuration.DatabaseName, _configuration.Username, _configuration.Password);

                _cachedMongoClient = new MongoClient(settings);

                return _cachedMongoClient;
            }
        }

        lock (_lock)
        {
            var pkiSettings = MongoClientSettings.FromUrl(new MongoUrl(_configuration.ConnectionString));
            pkiSettings.Credential = MongoCredential.CreateMongoX509Credential(username: null);
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

    public MongoConfigurationRecord GetMongoConfigurationRecord()
    {
        return _configuration;
    }
}
