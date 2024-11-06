using System.Reflection;
using System.Text.Json.Serialization;
using ChatBot.Api.Application.Abstractions;
using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Domain.PromptEntity;
using ChatBot.Api.EntityFrameworkCore.Postgresql;
using ChatBot.Api.EntityFrameworkCore.SqlServer;
using ChatBot.Api.Infrastructure;
using ChatBot.Api.Infrastructure.Repositories;
using ChatBot.Api.Infrastructure.Repositories.Mappers;
using ChatBot.Api.Swagger.Filters;
using Common.Cors;
using Common.HttpClient;
using Common.Mongo;
using Common.OpenAI.Clients;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddLogging(builder => builder.AddConsole());

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

    services.AddSingletonMongoClientFactory(configuration);
    // services.AddSingleton<IChatContextRepository, ChatContextMongoRepository>();

    services.AddDbContext<ChatBotContext>(builder =>
    {
        switch (configuration.GetValue<string>("DatabaseProvider"))
        {
            case DatabaseProviders.Postgresql:
                builder.UsePostgresqlDbContext(configuration.GetConnectionString(ConnectionStrings.ChatBotContextPostgresqlConnectionString)!);
                break;
            case DatabaseProviders.SqlServer:
            default:
                builder.UseSqlServerDbContext(configuration.GetConnectionString(ConnectionStrings.ChatBotContextSqlServerConnectionString)!);
                break;
            // case DatabaseProviders.MySql:
            //     builder.UseMySqlDbContext(configuration.GetConnectionString("ChatBotContext")!);
            //     break;
            // case DatabaseProviders.InMemory:
            //     builder.UseInMemoryDbContext("ChatBotContext");
            //     break;
        }
    });
    
    services.AddScoped<IChatContextRepository, ChatContextEntityFrameworkRepository>();

    services.AddMemoryCache();

    services.AddSingleton<IPromptRepository, PromptMongoRepository>()
        .Decorate<IPromptRepository, CachedUserAccessiblePromptRepository>();
}

public static class DatabaseProviders
{
    public const string SqlServer = nameof(SqlServer);
    public const string Postgresql = nameof(Postgresql);
    public const string MySql = "MySql";
    public const string InMemory = "InMemory";
}

public static class ConnectionStrings
{
    public static string ChatBotContextPostgresqlConnectionString = nameof(ChatBotContextPostgresqlConnectionString);
    public static string ChatBotContextSqlServerConnectionString = nameof(ChatBotContextSqlServerConnectionString);
}
