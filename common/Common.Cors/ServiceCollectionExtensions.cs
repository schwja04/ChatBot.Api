using Common.Cors.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Cors;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(CorsConfiguration.RootKey);
        var config = section.Get<CorsConfiguration>()!;

        //configuration.Bind(config.PolicyName, config);

        var policy = new CorsPolicyBuilder()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(config.AllowedOrigins)
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .Build();

        services.AddCors(options =>
        {
            options.AddPolicy(config.PolicyName, policy);
        });

        services.Configure<CorsConfiguration>(section);

        return services;
    }
}
