using AutoFixture;
using ChatBot.Api.Abstractions;
using ChatBot.Api.IntegrationTests.WebApplicationFactories.MockImplementations;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.EntityFrameworkCore.SqlServer;
using ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

namespace ChatBot.Api.IntegrationTests.WebApplicationFactories;

// ReSharper disable ClassNeverInstantiated.Global
public class SqlServerWebApplicationFactory :
    WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/azure-sql-edge")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
        .WithPortBinding(1433, true)
        .WithLogger(NullLogger.Instance)
        .Build();
    
    public Fixture Fixture { get; } = new();
    public HttpClient HttpClient { get; private set; } = null!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _msSqlContainer.GetConnectionString();
        
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>()
            {
                ["DatabaseProvider"] = "SqlServer",
                ["ConnectionStrings:ChatBotContextSqlServerConnectionString"] = connectionString,
                ["ChatCompletionOptions:Model"] = "llama3.2"
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
                contextBuilder.UseSqlServerDbContext(
                    config.GetConnectionString(ConnectionStrings.ChatBotContextSqlServerConnectionString)!);
            });
        });

        builder.ConfigureLogging(loggerBuilder => loggerBuilder.ClearProviders());
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IPromptRepository>();
            services.RemoveAll<IChatContextRepository>();
            services.RemoveAll<IChatClient>();
            
            services.AddScoped<IChatContextRepository, ChatContextEntityFrameworkRepository>();
            services.AddScoped<IPromptRepository, PromptEntityFrameworkRepository>();
            // services.Decorate<IPromptRepository, CachedPromptRepository>();
            services.AddSingleton<IChatClient, SubstituteChatClient>();
            
            services.AddAuthentication(defaultScheme: "TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "TestScheme", options => { });
        });
    }
    
    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        using (var serviceScope = this.Services.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ChatBotDbContext>();
            await context.Database.MigrateAsync();
        }
        
        HttpClient = CreateClient(new WebApplicationFactoryClientOptions
        {
            // BaseAddress = new Uri("https://localhost:5001")
        });
    }

    public new async Task DisposeAsync()
    {
        HttpClient.Dispose();
        await _msSqlContainer.StopAsync();
        await _msSqlContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}