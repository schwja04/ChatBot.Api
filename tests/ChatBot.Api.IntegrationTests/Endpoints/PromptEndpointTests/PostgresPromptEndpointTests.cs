using AutoFixture;
using ChatBot.Api.IntegrationTests.WebApplicationFactories;

namespace ChatBot.Api.IntegrationTests.Endpoints.PromptEndpointTests;

[Collection("PostgresCollection")]
public class PostgresPromptEndpointTests(PostgresWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.HttpClient;
    private readonly Fixture _fixture = factory.Fixture;

    [Fact]
    public async Task PromptEndpointsShould()
    {
        await PromptTestCases.RunAsync(_client, _fixture);
    }
}