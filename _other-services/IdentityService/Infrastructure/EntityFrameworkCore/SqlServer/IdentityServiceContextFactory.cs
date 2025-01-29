using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityService.Infrastructure.EntityFrameworkCore.SqlServer;

public class IdentityServiceContextFactory : IDesignTimeDbContextFactory<IdentityServiceContext>
{
    public IdentityServiceContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<IdentityServiceContext>();
        
        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("IdentityContextSqlServerConnectionString"),
            builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
        
        return new IdentityServiceContext(optionsBuilder.Options);
    }
}