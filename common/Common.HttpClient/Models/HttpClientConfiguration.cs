using Microsoft.Extensions.Configuration;

namespace Common.HttpClient.Models;

public class HttpClientConfiguration<TImplementationType>(IConfiguration configuration)
    : HttpClientConfiguration(typeof(TImplementationType).Name, configuration);

public class HttpClientConfiguration
{
    protected HttpClientConfiguration(string clientTypeName, IConfiguration configuration)
    {
        var clientSection = configuration.GetSection($"Services:{clientTypeName}");
        if (!clientSection.Exists())
        {
            throw new ArgumentNullException(clientTypeName, $"No child configuration section found in 'Services' for {clientTypeName}.");
        }

        Uri? baseAddress = clientSection.GetValue<Uri>("BaseAddress");
        if (baseAddress is not null && !baseAddress.AbsoluteUri.EndsWith("/"))
        {
            baseAddress = new Uri($"{baseAddress.AbsoluteUri}/");
        }
        BaseAddress = baseAddress!;

        ClientName = clientTypeName;
    }

    public string ClientName { get; }
    public Uri BaseAddress { get; }
}
