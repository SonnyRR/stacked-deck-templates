using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StackedDeck.Persistence.Extensions;

/// <summary>
/// Extension methods for configuring persistence services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds persistence services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

#if (UseMssqlProvider)
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(connectionString));
#elif (UsePostgresProvider)
        services.AddDbContext<ApplicationDbContext>(opt => opt
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());
#elif (UseSqliteProvider)
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite(connectionString));
#endif

        return services;
    }
}
