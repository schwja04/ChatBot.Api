using ChatBot.Api.IntegrationTests.WebApplicationFactories;

namespace ChatBot.Api.IntegrationTests.CollectionFixtures;

[CollectionDefinition("SqlServerCollection")]
public class SqlServerCollectionFixture : ICollectionFixture<SqlServerWebApplicationFactory>
{
}