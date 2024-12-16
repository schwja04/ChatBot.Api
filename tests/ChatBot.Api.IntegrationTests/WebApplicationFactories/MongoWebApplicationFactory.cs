using AutoFixture;
using ChatBot.Api.Abstractions;
using ChatBot.Api.IntegrationTests.WebApplicationFactories.MockImplementations;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.PromptEntity;
using Common.OpenAI.Clients;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace ChatBot.Api.IntegrationTests.WebApplicationFactories;

// ReSharper disable ClassNeverInstantiated.Global
public sealed class MongoWebApplicationFactory 
    : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private const string MongoPassword = "mySuperSecretPassword123!";
    
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder()
        .WithPassword(MongoPassword)
        .WithPortBinding(27017, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(27017))
        .Build();

    public Fixture Fixture { get; } = new();
    public HttpClient HttpClient { get; private set; } = null!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var mongoConnString = _mongoDbContainer.GetConnectionString();
        var mongoSettings = MongoClientSettings.FromConnectionString(mongoConnString);
        
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>()
            {
                ["DatabaseProvider"] = "Mongo",
                ["Mongo:ConnectionString"] = mongoConnString,
                ["Mongo:Username"] = mongoSettings.Credential!.Username,
                ["Mongo:Password"] = MongoPassword,
                ["Mongo:DatabaseName"] = "ChatBot",
                // ["Services:OpenAIClient:BaseAddress"] = OpenAIServer.BaseAddress,
            })
            .Build();
        
        builder.UseEnvironment("IntegrationTests");
        
        builder.ConfigureAppConfiguration(configBuilder =>
        {
            configBuilder.AddConfiguration(config);
        });
        
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IPromptRepository>();
            services.RemoveAll<IChatContextRepository>();
            services.RemoveAll<OpenAIClient>();
            
            services.AddMongoRepositories(config);
            // services.Decorate<IPromptRepository, CachedPromptRepository>();
            services.AddSingleton<IOpenAIClient, SubstituteOpenAIClient>();
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
        HttpClient = CreateClient(new WebApplicationFactoryClientOptions
        {
            // BaseAddress = new Uri("https://localhost:5001")
        });
    }

    public new async Task DisposeAsync()
    {
        HttpClient.Dispose();
        await _mongoDbContainer.StopAsync();
        await _mongoDbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}