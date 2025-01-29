using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var mongoConfig = builder.Configuration.GetRequiredSection("Mongo");
var mongoUsername = mongoConfig["Username"]!;
var mongoPassword = mongoConfig["Password"]!;
var databaseName = mongoConfig["DatabaseName"]!;

var username = builder.AddParameter("username", mongoUsername);
var password = builder.AddParameter("password", mongoPassword);

var mongo = builder
    .AddMongoDB(
        name: "mongo",
        userName: username,
        password: password
        )
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithMongoExpress();
var mongoDb = mongo.AddDatabase(databaseName);


var identityService = builder.AddProject<IdentityService>("identity-service");
identityService.WithReference(identityService);
identityService.WithEnvironment("Jwt__Issuer", "https://localhost:7159");

var openAIServiceHttp = builder.Configuration.GetValue<string>("Services:OpenAIService:http:0")!;
var proxyOpenAIService = builder.AddProject<ProxyOpenAIService>("proxy-openaiservice")
    .WithReference(identityService)
    .WithEnvironment("ReverseProxy__Clusters__openAIServiceCluster__Destinations__destination1", openAIServiceHttp)
    .WithEnvironment("Jwt__Issuer", "https://localhost:7159")
    .WithEnvironment("Jwt__Authority", "https://localhost:7159");

builder.AddProject<ChatBot_Api>("chatbot-api")
    .WithReference(proxyOpenAIService)
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithEnvironment("Mongo__ConnectionString", mongoDb.Resource.ConnectionStringExpression)
    .WithEnvironment("Mongo__Username", username.Resource.Value)
    .WithEnvironment("Mongo__Password", password.Resource.Value)
    .WithEnvironment("Mongo__DatabaseName", databaseName)
    .WithEnvironment("Services__OpenAIService__http__0", $"http://{proxyOpenAIService.Resource.Name}")
    .WithEnvironment("Services__OpenAIService__https__0", $"https://{proxyOpenAIService.Resource.Name}");



builder.Build().Run();
