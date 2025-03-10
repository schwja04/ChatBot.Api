using ChatBot.Api.ExceptionHandlers.ChatContextExceptionHandlers;
using ChatBot.Api.ExceptionHandlers.PromptExceptionHandlers;
using ChatBot.Application.Abstractions;
using ChatBot.Application.Abstractions.Repositories;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.EntityFrameworkCore.Postgresql;
using ChatBot.Infrastructure.EntityFrameworkCore.SqlServer;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;
using ChatBot.Infrastructure.Repositories.Persistence.Cached;
using ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using ChatBot.Infrastructure.Repositories.Persistence.InMemory;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo;
using Common.HttpClient;
using Common.Mongo;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace ChatBot.Api;

internal static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterExceptionHandlers();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMediatrRegistration>());

        services
            .AddOptions<ChatCompletionOptions>()
            .Bind(configuration.GetSection(nameof(ChatCompletionOptions)));

        services
            .AddTransient<IChatCompletionRepository, ChatCompletionRepository>()
            .AddSingleton<IPromptMessageMapper, PromptMessageMapper>()
            .AddSingleton<IChatMessageMapper, ChatMessageMapper>()
            .AddTransientWithHttpClient<IChatClient, OllamaChatClient>(configuration, (httpClient, sp) =>
            {
                var chatCompletionOptions = sp.GetRequiredService<IOptions<ChatCompletionOptions>>().Value;
                
                return new OllamaChatClient(httpClient.BaseAddress!, chatCompletionOptions.DefaultModel, httpClient);
            })
            .AddServiceDiscovery();

        
        var databaseProvider = configuration.GetValue<string>("DatabaseProvider");
        switch (databaseProvider)
        {
            case DatabaseProviders.SqlServer:
                services.AddSqlServerRepositories(configuration);
                break;
            case DatabaseProviders.Postgresql:
                services.AddPostgresqlRepositories(configuration);
                break;
            case DatabaseProviders.Mongo:
                services.AddMongoRepositories(configuration);
                break;
            case DatabaseProviders.InMemory:
            default:
                services.AddInMemoryRepositories();
                break;
        }

        services.AddMemoryCache()
            .Decorate<IPromptRepository, CachedPromptRepository>();
    }
    
    public static void RegisterExceptionHandlers(this IServiceCollection services)
    {
        services.AddProblemDetails();
        
        // Prompt exception handlers
        services.AddExceptionHandler<PromptAuthorizationExceptionHandler>();
        services.AddExceptionHandler<PromptDuplicateKeyExceptionHandler>();
        services.AddExceptionHandler<PromptKeyCannotBeEmptyExceptionHandler>();
        services.AddExceptionHandler<PromptNotFoundExceptionHandler>();
        
        // ChatContext exception handlers
        services.AddExceptionHandler<ChatContextAuthorizationExceptionHandler>();
        services.AddExceptionHandler<ChatContextNotFoundExceptionHandler>();
        services.AddExceptionHandler<ChatContextTitleCannotBeEmptyExceptionHandler>();
    }
    
    public static IServiceCollection AddSqlServerRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatBotDbContext>(builder =>
        {
            builder.UseSqlServerDbContext(configuration.GetConnectionString(ConnectionStrings.ChatBotContextSqlServerConnectionString)!);
        });

        services.AddScoped<IChatContextRepository, ChatContextEntityFrameworkRepository>();
        services.AddScoped<IPromptRepository, PromptEntityFrameworkRepository>();

        return services;
    }

    public static IServiceCollection AddPostgresqlRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatBotDbContext>(builder =>
        {
            builder.UsePostgresqlDbContext(configuration.GetConnectionString(ConnectionStrings.ChatBotContextPostgresqlConnectionString)!);
        });

        services.AddScoped<IChatContextRepository, ChatContextEntityFrameworkRepository>();
        services.AddScoped<IPromptRepository, PromptEntityFrameworkRepository>();

        return services;
    }

    public static IServiceCollection AddMongoRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingletonMongoClientFactory(configuration);
        services.AddSingleton<IChatContextRepository, ChatContextMongoRepository>();
        services.AddSingleton<IPromptRepository, PromptMongoRepository>();

        return services;
    }

    public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IChatContextRepository, ChatContextInMemoryRepository>();
        services.AddSingleton<IPromptRepository, PromptInMemoryRepository>();

        return services;
    }
}

internal static class DatabaseProviders
{
    public const string SqlServer = nameof(SqlServer);
    public const string Postgresql = nameof(Postgresql);
    public const string Mongo = nameof(Mongo);
    public const string InMemory = nameof(InMemory);
}

internal static class ConnectionStrings
{
    public const string ChatBotContextPostgresqlConnectionString = nameof(ChatBotContextPostgresqlConnectionString);
    public const string ChatBotContextSqlServerConnectionString = nameof(ChatBotContextSqlServerConnectionString);
}