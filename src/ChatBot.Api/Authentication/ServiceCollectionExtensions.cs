namespace ChatBot.Api.Authentication;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeycloakAuth(
        this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakSection = configuration.GetSection("Authorization:Keycloak");
        
        services
            .AddOptions<KeycloakConfigurationRecord>()
            .Bind(keycloakSection)
            .ValidateDataAnnotations();

        var config = keycloakSection.Get<KeycloakConfigurationRecord>();
        if (config is null)
        {
            throw new InvalidOperationException("Keycloak configuration is missing");
        }
        
        services.AddAuthentication()
            .AddKeycloakJwtBearer("keycloak", realm: config.Realm, options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = config.Audience;
            });
        
        return services;
    }
}