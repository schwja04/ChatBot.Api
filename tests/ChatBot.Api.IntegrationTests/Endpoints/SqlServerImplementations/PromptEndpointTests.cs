using AutoFixture;
using ChatBot.Api.IntegrationTests.Endpoints.Helpers;
using ChatBot.Api.IntegrationTests.WebApplicationFactories;

namespace ChatBot.Api.IntegrationTests.Endpoints.SqlServerImplementations;

public class PromptEndpointTests(SqlServerWebApplicationFactory factory) 
    : IClassFixture<SqlServerWebApplicationFactory>
{
    private readonly HttpClient _client = factory.HttpClient;
    private readonly Fixture _fixture = factory.Fixture;

    [Fact]
    public async Task PromptEndpointsShould()
    {
        await PromptTestCases.RunAsync(_client, _fixture);
    }
}