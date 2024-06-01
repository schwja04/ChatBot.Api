using ChatBot.Api.Application.Abstractions;
using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Infrastructure.Repositories;
using Common.HttpClient;
using Common.Mongo;
using Common.OpenAI.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
RegisterServices(builder.Services, builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();

static void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMediatrRegistration>());

    services.AddTransientWithHttpClient<IOpenAIClient, OpenAIClient>(configuration);
    services.AddTransient<IChatCompletionRepository, ChatCompletionRepository>();
    // services.AddSingleton<IChatHistoryRepository, ChatHistoryInMemoryRepository>();

    services.AddSingletonMongoClientFactory(configuration);
    services.AddSingleton<IChatHistoryRepository, ChatHistoryMongoRepository>();
}

