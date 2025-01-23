using Microsoft.Extensions.Configuration;

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

var openAIServiceConfig = builder.Configuration.GetRequiredSection("Services:OpenAIService");
var openAIServiceHttp = openAIServiceConfig["http"]!;

builder.AddProject<Projects.ChatBot_Api>("chatbot-api")
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithEnvironment("Mongo__ConnectionString", mongoDb.Resource.ConnectionStringExpression)
    .WithEnvironment("Mongo__Username", username.Resource.Value)
    .WithEnvironment("Mongo__Password", password.Resource.Value)
    .WithEnvironment("Mongo__DatabaseName", databaseName)
    .WithEnvironment("Services__OpenAIService__http", openAIServiceHttp);

builder.Build().Run();
