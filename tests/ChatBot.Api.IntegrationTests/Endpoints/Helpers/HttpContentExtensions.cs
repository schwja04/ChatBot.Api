using Newtonsoft.Json;

namespace ChatBot.Api.IntegrationTests.Endpoints.Helpers;

internal static class HttpContentExtensions
{
    public static async Task<T?> DeserializeAsync<T>(this HttpContent content)
        where T : class
    {
        var responseString = await content.ReadAsStringAsync();
        
        if (string.IsNullOrWhiteSpace(responseString))
        {
            throw new InvalidOperationException("Response was empty");
        }
        
        return JsonConvert.DeserializeObject<T>(responseString);
    }
}