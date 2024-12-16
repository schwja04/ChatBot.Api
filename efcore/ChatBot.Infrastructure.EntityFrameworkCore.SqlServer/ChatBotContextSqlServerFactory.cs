using System.Reflection;
using ChatBot.Api.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ChatBot.Api.EntityFrameworkCore.SqlServer;

public class ChatBotContextSqlServerFactory : IDesignTimeDbContextFactory<ChatBotDbContext>
{
    public ChatBotDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.Development.json")
            .Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<ChatBotDbContext>();
        
        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("ChatBotContext"),
            builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
        
        return new ChatBotDbContext(optionsBuilder.Options);
    }
}