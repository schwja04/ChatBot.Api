using Microsoft.EntityFrameworkCore;

namespace ChatBot.Api.EntityFrameworkCore.Postgresql;

public static class ServiceCollectionExtensions
{
    public static DbContextOptionsBuilder UsePostgresqlDbContext(
        this DbContextOptionsBuilder optionsBuilder, 
        string connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString, sqlServerOptionsAction =>
        {
            sqlServerOptionsAction.MigrationsAssembly(typeof(ChatBotContextPostgresqlFactory).Assembly.FullName);
        });

        return optionsBuilder;
    }
}