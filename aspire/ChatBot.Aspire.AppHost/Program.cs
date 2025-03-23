using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var mongoConfig = builder.Configuration.GetRequiredSection("Mongo");
var databaseName = mongoConfig["DatabaseName"]!;

var mongoUsername = builder.AddParameter("username", mongoConfig["Username"]!);
var mongoPassword = builder.AddParameter("password", mongoConfig["Password"]!);

var mongo = builder
    .AddMongoDB(
        name: "mongo",
        userName: mongoUsername,
        password: mongoPassword
        )
    .WithEndpoint(port: 27017, targetPort: 27017, scheme: "tcp", isProxied: false, name: "mongo")
    .WithExternalHttpEndpoints()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithMongoExpress();
var mongoDb = mongo.AddDatabase(databaseName);

var keycloakConfig = builder.Configuration.GetRequiredSection("Keycloak");
var keycloakUsername = builder.AddParameter("bootstrapUser", keycloakConfig["BootstrapUser"]!);
var keycloakPassword = builder.AddParameter("bootstrapPassword", keycloakConfig["BootstrapPassword"]!);

var keycloak = builder.AddKeycloak(
        "keycloak", 
        8080,
        keycloakUsername,
        keycloakPassword
        )
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithExternalHttpEndpoints();

var openAIServiceHttp = builder.Configuration.GetValue<string>("Services:OpenAIService:http:0")!;
var keycloakBaseAddress = keycloak.Resource.GetEndpoint("http");
builder.AddProject<ChatBot_Api>("chatbot-api")
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithEnvironment("Mongo__ConnectionString", mongoDb.Resource.ConnectionStringExpression)
    .WithEnvironment("Mongo__Username", mongoUsername.Resource.Value)
    .WithEnvironment("Mongo__Password", mongoPassword.Resource.Value)
    .WithEnvironment("Mongo__DatabaseName", databaseName)
    .WithEnvironment("Services__OpenAIService__http__0", openAIServiceHttp)
    .WithEnvironment("Authorization__Keycloak__BaseAddress", keycloakBaseAddress);

builder.Build().Run();
