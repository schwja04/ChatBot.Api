using ChatBot.Api.IntegrationTests.WebApplicationFactories;

namespace ChatBot.Api.IntegrationTests.CollectionFixtures;

[CollectionDefinition("PostgresCollection")]
public class PostgresCollectionFixture : ICollectionFixture<PostgresWebApplicationFactory>
{
}