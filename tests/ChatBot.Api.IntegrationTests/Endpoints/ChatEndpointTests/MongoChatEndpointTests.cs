using ChatBot.Api.IntegrationTests.WebApplicationFactories;

namespace ChatBot.Api.IntegrationTests.Endpoints.ChatEndpointTests;

[Collection("MongoCollection")]
public class MongoChatEndpointTests(MongoWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.HttpClient;
    
    [Fact]
    public async Task PromptEndpointsShould()
    {
        await ChatTestCases.RunAsync(_client);
    }
}