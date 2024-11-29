using System.Text.Json.Serialization;
using ChatBot.Api.Application.Abstractions;
using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Domain.PromptEntity;
using ChatBot.Api.EntityFrameworkCore.Postgresql;
using ChatBot.Api.EntityFrameworkCore.SqlServer;
using ChatBot.Api.Infrastructure;
using ChatBot.Api.Infrastructure.Repositories;
using ChatBot.Api.Infrastructure.Repositories.ExternalServices.ChatCompletion;
using ChatBot.Api.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Cached;
using ChatBot.Api.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using ChatBot.Api.Infrastructure.Repositories.Persistence.InMemory;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo;
using ChatBot.Api.Swagger.Filters;
using Common.Cors;
using Common.HttpClient;
using Common.Mongo;
using Common.OpenAI.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsConfiguration(builder.Configuration);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);


// Add services to the container.
RegisterServices(builder.Services, builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

// Add swagger with Newtonsoft functionality
builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("CorsPolicy");

app.MapControllers();
app.UseHttpsRedirection();

app.Run();

static void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMediatrRegistration>());

    services.AddTransientWithHttpClient<IOpenAIClient, OpenAIClient>(configuration);
    services.AddTransient<IChatCompletionRepository, ChatCompletionRepository>();
    services.AddSingleton<IPromptMessageMapper, PromptMessageMapper>();
    
    var databaseProvider = configuration.GetValue<string>("DatabaseProvider");
    switch (databaseProvider)
    {
        case DatabaseProviders.SqlServer:
            services.AddSqlServerRepositories(configuration);
            break;
        case DatabaseProviders.Postgresql:
            services.AddPostgresqlRepositories(configuration);
            break;
        case DatabaseProviders.InMemory:
            services.AddInMemoryRepositories();
            break;
        case DatabaseProviders.Mongo:
            services.AddMongoRepositories(configuration);
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(databaseProvider), databaseProvider, "Database provider is not supported.");
    }

    services.AddMemoryCache()
        .Decorate<IPromptRepository, CachedUserAccessiblePromptRepository>();
}

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatBotContext>(builder =>
        {
            builder.UseSqlServerDbContext(configuration.GetConnectionString(ConnectionStrings.ChatBotContextSqlServerConnectionString)!);
        });

        services.AddScoped<IChatContextRepository, ChatContextEntityFrameworkRepository>();
        services.AddScoped<IPromptRepository, PromptEntityFrameworkRepository>();

        return services;
    }
    
    public static IServiceCollection AddPostgresqlRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatBotContext>(builder =>
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
