using Data;
using Microsoft.EntityFrameworkCore;

namespace App;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection sc, IConfiguration config)
    {
        sc.AddDbContext<DataContext>(ConfigDrivenOptions(config));
        return sc;
    }

    private static Action<DbContextOptionsBuilder> ConfigDrivenOptions(IConfiguration config)
    {
        return config["DatabaseType"]?.ToLowerInvariant() switch
        {
            "pgsql" => (opts) => opts.UseNpgsql(config.GetConnectionString("pgsql"), x => x.MigrationsAssembly("PgsqlMigrations")),
            _ => (opts) => opts.UseSqlite(config.GetConnectionString("sqlite"), x => x.MigrationsAssembly("Migrations"))
        };
    }
}