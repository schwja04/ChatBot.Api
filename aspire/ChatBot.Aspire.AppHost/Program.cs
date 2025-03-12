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
    .WithEndpoint(port: 27017, targetPort: 27017, scheme: "tcp", isProxied: false, name: "mongo")
    .WithExternalHttpEndpoints()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithMongoExpress();
var mongoDb = mongo.AddDatabase(databaseName);

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithExternalHttpEndpoints();

var openAIServiceHttp = builder.Configuration.GetValue<string>("Services:OpenAIService:http:0")!;

builder.AddProject<ChatBot_Api>("chatbot-api")
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithEnvironment("Mongo__ConnectionString", mongoDb.Resource.ConnectionStringExpression)
    .WithEnvironment("Mongo__Username", username.Resource.Value)
    .WithEnvironment("Mongo__Password", password.Resource.Value)
    .WithEnvironment("Mongo__DatabaseName", databaseName)
    .WithEnvironment("Services__OpenAIService__http__0", openAIServiceHttp);

builder.Build().Run();
