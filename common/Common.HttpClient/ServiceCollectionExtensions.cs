using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MS = System.Net.Http;

using Common.HttpClient.Models;

namespace Common.HttpClient;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddTransientWithHttpClient<TInterfaceType, TImplementationType>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MS.HttpClient>? configureClient = null)
            where TInterfaceType : class
            where TImplementationType : class, TInterfaceType
    {
        HttpClientConfiguration<TImplementationType> httpClientConfiguration = new(configuration);

        return services.AddTransientWithHttpClient<TInterfaceType, TImplementationType>(httpClientConfiguration, configureClient);
    }

    public static IHttpClientBuilder AddTransientWithHttpClient<TInterfaceType, TImplementationType>(
        this IServiceCollection services,
        HttpClientConfiguration configuration,
        Action<MS.HttpClient>? configureClient = null)
        where TInterfaceType : class
        where TImplementationType : class, TInterfaceType
    {
        return services.AddHttpClient<TInterfaceType, TImplementationType>(configuration.ClientName, client =>
        {
            configureClient?.Invoke(client);

            client.BaseAddress = configuration.BaseAddress;
        });
    }
    
    public static IHttpClientBuilder AddTransientWithHttpClient<TInterfaceType, TImplementationType>(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<MS.HttpClient, IServiceProvider, TImplementationType> implementationFactory,
        Action<MS.HttpClient>? configureClient = null)
        where TInterfaceType : class
        where TImplementationType : class, TInterfaceType
    {
        HttpClientConfiguration<TImplementationType> httpClientConfiguration = new(configuration);

        return services.AddTransientWithHttpClient<TInterfaceType, TImplementationType>(httpClientConfiguration, implementationFactory);
    }
    
    public static IHttpClientBuilder AddTransientWithHttpClient<TInterfaceType, TImplementationType>(
        this IServiceCollection services,
        HttpClientConfiguration configuration,
        Func<MS.HttpClient, IServiceProvider, TImplementationType> implementationFactory)
        where TInterfaceType : class
        where TImplementationType : class, TInterfaceType
    {
        return services.AddHttpClient<TInterfaceType, TImplementationType>(
            configuration.ClientName, 
            (client, serviceProvider) =>
            {
                client.BaseAddress = configuration.BaseAddress;
                
                return implementationFactory(client, serviceProvider);
            });
    }
}
