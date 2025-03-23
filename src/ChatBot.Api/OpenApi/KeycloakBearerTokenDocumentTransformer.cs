using ChatBot.Api.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace ChatBot.Api.OpenApi;

internal class KeycloakBearerTokenDocumentTransformer(IServiceProvider serviceProvider) : IOpenApiDocumentTransformer
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        // 1. Get the Keycloak configuration from the service provider
        var keycloakConfig = _serviceProvider.GetRequiredService<IOptions<KeycloakConfigurationRecord>>().Value;
        
        // 2. Add the Security Scheme at the document level
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>()
        {
            ["oauth2"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    AuthorizationCode = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{keycloakConfig.BaseAddress.AbsoluteUri.TrimEnd('/')}/realms/{keycloakConfig.Realm}/protocol/openid-connect/auth"),
                        TokenUrl = new Uri($"{keycloakConfig.BaseAddress.AbsoluteUri.TrimEnd('/')}/realms/{keycloakConfig.Realm}/protocol/openid-connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            ["openid"] = "OpenID Connect scope",
                            ["profile"] = "Access user profile",
                            ["email"] = "Access user email"
                        }
                    },
                }
            }
        };
        
        // 3. Create the security requirement
        var securityRequirement = new OpenApiSecurityRequirement
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
                new[] { "openid", "profile", "email" }
            }
        };
        

        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
        {
            operation.Value.Security.Add(securityRequirement);
        }
        
        return Task.CompletedTask;
    }
}