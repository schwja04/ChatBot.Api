using ChatBot.Api.Authentication;

namespace ChatBot.Api.Swagger;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerGenWithKeycloak(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KeycloakConfigurationRecord>()
            .Bind(configuration.GetSection("Authorization:Keycloak"))
            .ValidateDataAnnotations();
            
        services
            .AddOpenApi(options =>
            {
                options.AddDocumentTransformer<KeyCloakBearerTokenDocumentTransformer>();
            });
        // .AddSingleton<KeyCloakAuthorizationFilter>()
        // .AddSwaggerGen(options =>
        // {
        //     options.SchemaFilter<EnumSchemaFilter>();
        //     options.OperationFilter<KeyCloakAuthorizationFilter>();
        // });
        return services;
    }
}