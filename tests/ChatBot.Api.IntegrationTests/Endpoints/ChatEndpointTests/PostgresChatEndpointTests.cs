using AutoFixture;
using ChatBot.Api.IntegrationTests.Endpoints.TestHelpers;
using ChatBot.Api.IntegrationTests.WebApplicationFactories;
using ChatBot.Api.IntegrationTests.WebApplicationFactories.MockImplementations;

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