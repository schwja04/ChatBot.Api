using ChatBot.Api.IntegrationTests.WebApplicationFactories;

namespace ChatBot.Api.IntegrationTests.CollectionFixtures;

[CollectionDefinition("SqlServerCollection")]
public class SqlServerCollection : ICollectionFixture<SqlServerWebApplicationFactory>
{
}