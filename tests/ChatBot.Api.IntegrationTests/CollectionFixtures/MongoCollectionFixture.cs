using ChatBot.Api.IntegrationTests.WebApplicationFactories;

namespace ChatBot.Api.IntegrationTests.CollectionFixtures;

[CollectionDefinition("MongoCollection")]
public class MongoCollectionFixture : ICollectionFixture<MongoWebApplicationFactory>
{
}