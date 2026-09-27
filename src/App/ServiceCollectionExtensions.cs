using Data;
using Microsoft.EntityFrameworkCore;

namespace App;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection sc, IConfiguration config)
    {
        sc.AddDbContext<DataContext>(
            (opts) => opts.UseNpgsql(
                config.GetConnectionString("pgsql"),
                x => x.MigrationsAssembly("PgsqlMigrations")
                )
            );
        return sc;
    }
}
