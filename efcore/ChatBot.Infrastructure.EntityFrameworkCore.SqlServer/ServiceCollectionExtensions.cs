using Microsoft.EntityFrameworkCore;

namespace ChatBot.Infrastructure.EntityFrameworkCore.SqlServer;

public static class ServiceCollectionExtensions
{
    public static DbContextOptionsBuilder UseSqlServerDbContext(
        this DbContextOptionsBuilder optionsBuilder, 
        string connectionString)
    {
        optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction =>
        {
            sqlServerOptionsAction.MigrationsAssembly(typeof(ChatBotContextSqlServerFactory).Assembly.FullName);
        });

        return optionsBuilder;
    }
}