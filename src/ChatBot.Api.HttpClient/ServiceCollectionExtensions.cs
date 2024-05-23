using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MS = System.Net.Http;

using ChatBot.Api.HttpClient.Models;

namespace ChatBot.Api.HttpClient;

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
        HttpClientConfiguration<TImplementationType> configuration,
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
}
