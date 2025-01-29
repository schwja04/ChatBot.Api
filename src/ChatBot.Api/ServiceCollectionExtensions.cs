using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.EntityFrameworkCore.Postgresql;
using ChatBot.Infrastructure.EntityFrameworkCore.SqlServer;
using ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using ChatBot.Infrastructure.Repositories.Persistence.InMemory;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo;
using Common.Mongo;

namespace ChatBot.Api;

internal static class ServiceCollectionExtensions
{
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

internal static class ConnectionStrings
{
    public const string ChatBotContextPostgresqlConnectionString = nameof(ChatBotContextPostgresqlConnectionString);
    public const string ChatBotContextSqlServerConnectionString = nameof(ChatBotContextSqlServerConnectionString);
}