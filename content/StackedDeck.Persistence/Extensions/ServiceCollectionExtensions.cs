using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StackedDeck.Persistence.Template.Interceptors;

namespace StackedDeck.Persistence.Template.Extensions;

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

        services.AddScoped<AuditInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, opt) =>
#if (UseMssqlProvider)
            opt.UseSqlServer(connectionString)
#elif (UsePostgresProvider)
            opt.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
#elif (UseSqliteProvider)
            opt.UseSqlite(connectionString)
#else
            opt.UseSqlite(connectionString)
#endif
#if (UseAuditNet)
                .AddInterceptors(
                    sp.GetRequiredService<SoftDeleteInterceptor>(),
                    sp.GetRequiredService<AuditInterceptor>()));
#else
                .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>()));
#endif

#if (UseAuditNet)
        Audit.Core.Configuration
            .Setup()
            .UseStackedDeckAuditing();
#endif

        return services;
    }
}
