using System.Net.Http.Headers;
using Common.HttpClient;
using Common.OpenAI.Clients;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChatBot.Api;

internal static class Scratch
{
    public static void AddOpenAIClientWithAuthCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ITokenProvider, MyTokenProvider>();

        services.AddMemoryCache()
            .Decorate<ITokenProvider, CachedTokenProvider>();

        services.RemoveAll<IOpenAIClient>();
            
        services
            .AddTransientWithHttpClient<IOpenAIClient, OpenAIClient>(configuration)
            .AddHttpMessageHandler<AuthMessageHandler>()
            .AddServiceDiscovery();
    }
}

internal class AuthMessageHandler(ITokenProvider tokenProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await tokenProvider.GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
        return await base.SendAsync(request, cancellationToken);
    }
}

internal interface ITokenProvider
{
    Task<MyAccessToken> GetTokenAsync(CancellationToken cancellationToken = default);
}

internal class CachedTokenProvider(ITokenProvider tokenProvider, IMemoryCache memoryCache) : ITokenProvider
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IMemoryCache _memoryCache = memoryCache;

    private const string AccessTokenCacheKey = "AccessToken";
    public async Task<MyAccessToken> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(AccessTokenCacheKey, out MyAccessToken? accessToken))
        {
            return accessToken!;
        }

        accessToken = await _tokenProvider.GetTokenAsync(cancellationToken);
        _memoryCache.Set(AccessTokenCacheKey, accessToken, TimeSpan.FromSeconds(accessToken.ExpiresIn));

        return accessToken;
    }
}

internal class MyTokenProvider(HttpClient httpClient, IConfiguration configuration) : ITokenProvider
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;

    public async Task<MyAccessToken> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("https://localhost:7159/auth/login", new
        {
            Username = "admin",
            Password = "Password123!"
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        var accessToken = await response.Content.ReadFromJsonAsync<MyAccessToken>(cancellationToken: cancellationToken);
        
        return accessToken!;
    }
}

internal record MyAccessToken
{
    public required string Token { get; init; }
    public required DateTimeOffset ExpiresOn { get; init; }
    public required long ExpiresIn { get; init; }
}