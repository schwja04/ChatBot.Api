using System.Text.Json.Serialization;
using ChatBot.Api;
using ChatBot.Api.Swagger;
using Common.Cors;
using Common.ServiceDefaults;
using Microsoft.IdentityModel.JsonWebTokens;

// ReSharper disable ClassNeverInstantiated.Global

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCorsConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.RegisterServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithKeycloak(builder.Configuration);

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

builder.Services.AddAuthorization(
    options =>
    {
        // options.FallbackPolicy = new AuthorizationPolicyBuilder()
        //     .RequireAuthenticatedUser()
        //     .Build();
    });
builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer("keycloak", realm: "chatbot", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = "account";
    });

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
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ChatBot API");
        options.OAuthClientId("chatbot-public-client");
        options.OAuthScopeSeparator(" ");
        options.OAuthAppName("Swagger UI with Keycloak");
        options.OAuth2RedirectUrl($"https://localhost:5001/swagger/oauth2-redirect.html");
    });
}

app.UseRouting();

app.UseCors("CorsPolicy");

app.MapControllers();
app.UseHttpsRedirection();
app.UseExceptionHandler();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
app.UseAuthentication();
app.UseAuthorization();

app.Run();