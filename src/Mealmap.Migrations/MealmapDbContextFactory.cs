using Mealmap.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Migrations;

public class MealmapDbContextFactory : IDesignTimeDbContextFactory<MealmapDbContext>
{
    public MealmapDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MealmapDbContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("MealmapDb"), b => b.MigrationsAssembly("Mealmap.Migrations"));

        return new MealmapDbContext(optionsBuilder.Options);
    }
}
