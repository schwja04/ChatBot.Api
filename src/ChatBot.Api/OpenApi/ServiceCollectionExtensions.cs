using ChatBot.Api.Authentication;

namespace ChatBot.Api.OpenApi;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeycloakWithOpenApiSupport(
        this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }
}