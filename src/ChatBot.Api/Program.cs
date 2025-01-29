using System.Text.Json.Serialization;
using ChatBot.Api;
using ChatBot.Api.ExceptionHandlers.ChatContextExceptionHandlers;
using ChatBot.Api.ExceptionHandlers.PromptExceptionHandlers;
using ChatBot.Api.Swagger.Filters;
using ChatBot.Application.Abstractions;
using ChatBot.Application.Abstractions.Repositories;
using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;
using ChatBot.Infrastructure.Repositories.Persistence.Cached;
using Common.Cors;
using Common.HttpClient;
using Common.OpenAI.Clients;
using Common.ServiceDefaults;

// ReSharper disable ClassNeverInstantiated.Global

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCorsConfiguration(builder.Configuration);

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
    
    services.AddTransientWithHttpClient<IOpenAIClient, OpenAIClient>(configuration)
        .AddServiceDiscovery();
    
    services.AddTransient<IChatCompletionRepository, ChatCompletionRepository>();
    services.AddSingleton<IPromptMessageMapper, PromptMessageMapper>();
    services.AddMongoRepositories(configuration);

    services.AddOpenAIClientWithAuthCaching(configuration);
    
    services.AddMemoryCache()
        .Decorate<IPromptRepository, CachedPromptRepository>();
}