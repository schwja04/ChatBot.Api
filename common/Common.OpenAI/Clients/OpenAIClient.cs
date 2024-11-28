using Newtonsoft.Json;
using System.Net.Http.Json;

using Common.OpenAI.Models;

namespace Common.OpenAI.Clients;

public class OpenAIClient(HttpClient httpClient) : IOpenAIClient
{
    private static readonly Uri _chatCompletionUri = new Uri("chat/completions", UriKind.Relative);
    private readonly HttpClient _httpClient = httpClient;
    
    public async Task<CreateChatCompletionResponse> CreateChatCompletionAsync(
            CreateChatCompletionRequest request,
            CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_chatCompletionUri, request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to create chat completion. Status code: {response.StatusCode}. Content: {content}");
        }

        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonConvert.DeserializeObject<CreateChatCompletionResponse>(responseContent)!;
    }
}
