using System.Text.Json.Serialization;
using ChatBot.Api;
using ChatBot.Api.Authentication;
using ChatBot.Api.OpenApi;
using Common.Cors;
using Common.ServiceDefaults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;

// ReSharper disable ClassNeverInstantiated.Global

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCorsConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.RegisterServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<KeycloakBearerTokenDocumentTransformer>();
    // THIS IS A WORKAROUND, THIS SHOULD BE REMOVED AT THE RELEASE OF 9.0.4
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Servers = new List<OpenApiServer>();
        return Task.CompletedTask;
    });
});

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

builder.Services.AddAuthorization(
    options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });
builder.Services.AddKeycloakAuth(builder.Configuration);

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
    app.MapOpenApi().AllowAnonymous();
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