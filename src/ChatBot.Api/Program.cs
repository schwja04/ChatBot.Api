using System.Text.Json;
using System.Text.Json.Serialization;
using ChatBot.Api;
using ChatBot.Api.Swagger.Filters;
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var keycloakConfig = builder.Configuration.GetSection("Authorization:Keycloak");
    var realm = keycloakConfig["Realm"]!;
    
    
    using var httpClient = new HttpClient();
    var json = httpClient.GetStringAsync($"https+http://keycloak/realms/{realm}/.well-known/openid-configuration").Result;
    var config = JsonSerializer.Deserialize<OpenIdConnectConfiguration>(json)!;
    
    options.SchemaFilter<EnumSchemaFilter>();

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(config.AuthorizationEndpoint),
                TokenUrl = new Uri(config.TokenEndpoint),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect scope" },
                    { "profile", "Access user profile" },
                    { "email", "Access user email" }
                },
                
            },
            
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new List<string> { "openid", "profile", "email" }
        }
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
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("chatbot-public-client");
        options.OAuthScopeSeparator(" ");
        options.OAuthAppName("Swagger UI with Keycloak");
        options.OAuth2RedirectUrl("https://localhost:5001/swagger/oauth2-redirect.html");
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

public class OpenIdConnectConfiguration
{
    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; } = null!;
    
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = null!;
    
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = null!;
}