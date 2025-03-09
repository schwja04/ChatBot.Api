using ChatBot.Api.IntegrationTests.WebApplicationFactories;

namespace ChatBot.Api.IntegrationTests.Endpoints.ChatEndpointTests;

[Collection("PostgresCollection")]
public class PostgresChatEndpointTests(PostgresWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.HttpClient;
    
    [Fact]
    public async Task PromptEndpointsShould()
    {
        await ChatTestCases.RunAsync(_client);
    }
}