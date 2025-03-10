using System.Text.Json.Serialization;
using ChatBot.Api;
using ChatBot.Api.Swagger.Filters;
using Common.Cors;
using Common.ServiceDefaults;

// ReSharper disable ClassNeverInstantiated.Global

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCorsConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.RegisterServices(builder.Configuration);

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