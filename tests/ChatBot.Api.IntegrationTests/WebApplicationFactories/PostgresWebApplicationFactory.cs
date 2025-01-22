using AutoFixture;
using ChatBot.Api.Abstractions;
using ChatBot.Api.IntegrationTests.WebApplicationFactories.MockImplementations;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.EntityFrameworkCore.Postgresql;
using ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using Common.OpenAI.Clients;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace ChatBot.Api.IntegrationTests.WebApplicationFactories;

// ReSharper disable ClassNeverInstantiated.Global
public class PostgresWebApplicationFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase("chatbot")
        .WithUsername("chatbot")
        .WithPassword("chatbot")
        .WithPortBinding(5432, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();
    
    public Fixture Fixture { get; } = new();
    public HttpClient HttpClient { get; private set; } = null!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _postgresContainer.GetConnectionString();
        
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>()
            {
                ["DatabaseProvider"] = "Postgres",
                ["ConnectionStrings:ChatBotContextPostgresqlConnectionString"] = connectionString,
                //["Services:OpenAIService:http"] = OpenAIServer.BaseAddress,
            })
            .Build();
        
        builder.UseEnvironment("IntegrationTests");
        
        builder.ConfigureAppConfiguration(configBuilder =>
        {
            configBuilder.AddConfiguration(config);
        });

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<ChatBotDbContext>(contextBuilder =>
            {
                contextBuilder.UsePostgresqlDbContext(
                    config.GetConnectionString(ConnectionStrings.ChatBotContextPostgresqlConnectionString)!);
            });
        });

        builder.ConfigureLogging(loggerBuilder => loggerBuilder.ClearProviders());
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IPromptRepository>();
            services.RemoveAll<IChatContextRepository>();
            services.RemoveAll<OpenAIClient>();
            
            services.AddScoped<IPromptRepository, PromptEntityFrameworkRepository>();
            services.AddScoped<IChatContextRepository, ChatContextEntityFrameworkRepository>();
            // services.Decorate<IPromptRepository, CachedPromptRepository>();
            services.AddSingleton<IOpenAIClient, SubstituteOpenAIClient>();
        });
    }
    
    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        using (var serviceScope = this.Services.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ChatBotDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        
        HttpClient = CreateClient(new WebApplicationFactoryClientOptions()
        {
            // BaseAddress = new Uri("https://localhost:5001")
        });
    }

    public new async Task DisposeAsync()
    {
        HttpClient.Dispose();
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}