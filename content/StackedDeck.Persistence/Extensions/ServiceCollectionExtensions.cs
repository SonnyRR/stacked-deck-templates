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
    /// <param name="connectionString">The SQL Server connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPersistenceService(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(connectionString));

        return services;
    }
}
