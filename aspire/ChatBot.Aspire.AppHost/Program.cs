var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ChatBot_Api>("chatbot-api");

builder.Build().Run();
