using Common.Mongo.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Common.Mongo;

public static class ServiceCollectionExtensions
{
    private const string ActivityNameSource = "MongoDB.Driver.Core.Extensions.DiagnosticSources";
    
    public static IServiceCollection AddSingletonMongoClientFactory(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IMongoDatabase>? setupDatabase = null)
    {
        if (setupDatabase is not null)
        {
            SetupDatabase(configuration, setupDatabase);
        }

        services
            .BindMongoConfigurationOptions(configuration)
            .BindPkiConfigurationOptions(configuration)
            .AddSingleton<IMongoConfigManager, MongoConfigManager>()
            .AddSingleton<IX509Manager, X509Manager>()
            .AddSingleton<IMongoClientFactory, MongoClientFactory>();
        
        services.AddOpenTelemetry()
            .WithTracing(tracer => tracer.AddSource(ActivityNameSource));

        return services;
    }

    public static IServiceCollection BindMongoConfigurationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoSection = configuration.GetSection(MongoConfigurationRecord.RootKey);

        if (!mongoSection.Exists())
        {
            throw new InvalidOperationException($"Missing configuration section: {MongoConfigurationRecord.RootKey}");
        }

        services
            .AddOptions<MongoConfigurationRecord>()
            .Bind(mongoSection)
            .ValidateDataAnnotations();

        return services;
    }

    public static IServiceCollection BindPkiConfigurationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var pkiSection = configuration.GetSection(PkiConfigurationRecord.RootKey);

        // if (!pkiSection.Exists())
        // {
        //     throw new InvalidOperationException($"Missing configuration section: {PkiConfigurationRecord.RootKey}");
        // }

        services
            .AddOptions<PkiConfigurationRecord>()
            .Bind(pkiSection);

        return services;
    }

    private static void SetupDatabase(IConfiguration configuration, Action<IMongoDatabase> setupDatabase)
    {
        MongoConfigurationRecord mongoConfigurationRecord = new();
        configuration.Bind(MongoConfigurationRecord.RootKey, mongoConfigurationRecord);
        MongoConfigManager mongoConfigManager = new(mongoConfigurationRecord);

        PkiConfigurationRecord pkiConfigurationRecord = new();
        configuration.Bind(PkiConfigurationRecord.RootKey, pkiConfigurationRecord);
        X509Manager x509Manager = new(pkiConfigurationRecord);

        MongoClientFactory mongoClientFactory = new MongoClientFactory(mongoConfigManager, x509Manager);

        IMongoDatabase database = mongoClientFactory
            .CreateClient()
            .GetDatabase(mongoConfigurationRecord.DatabaseName);

        setupDatabase.Invoke(database);
    }
}
