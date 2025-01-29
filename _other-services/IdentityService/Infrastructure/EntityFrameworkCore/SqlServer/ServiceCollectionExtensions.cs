using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.EntityFrameworkCore.SqlServer;

public static class ServiceCollectionExtensions
{
    public static DbContextOptionsBuilder UseSqlServerDbContext(
        this DbContextOptionsBuilder optionsBuilder, 
        string connectionString)
    {
        optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction =>
        {
            sqlServerOptionsAction.MigrationsAssembly(typeof(IdentityServiceContextFactory).Assembly.FullName);
        });

        return optionsBuilder;
    }
}