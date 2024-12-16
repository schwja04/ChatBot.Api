using System.Text.Json.Serialization;
using ChatBot.Api;
using ChatBot.Api.ExceptionHandlers.ChatContextExceptionHandlers;
using ChatBot.Api.ExceptionHandlers.PromptExceptionHandlers;
using ChatBot.Api.Swagger.Filters;
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
using Common.Cors;
using Common.HttpClient;
using Common.Mongo;
using Common.OpenAI.Clients;
using Common.ServiceDefaults;

// ReSharper disable ClassNeverInstantiated.Global

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCorsConfiguration(builder.Configuration);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);


// Add services to the container.
RegisterServices(builder.Services, builder.Configuration);
RegisterExceptionHandlers(builder.Services);

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
app.UseExceptionHandler();

app.Run();

static void RegisterExceptionHandlers(IServiceCollection services)
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

namespace ChatBot.Api
{
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
}