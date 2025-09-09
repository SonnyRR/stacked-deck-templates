using System;

using Microsoft.Extensions.DependencyInjection;

using StackedDeck.WebAPI.Template.Data.Interfaces;
using StackedDeck.WebAPI.Template.Data.Repositories;

namespace StackedDeck.WebAPI.Template.Data.Extensions;

/// <summary>
/// Service collection extensions for registering persistence services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers persistence application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The connection string for the database resource.</param>
    /// <returns>The service collection.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        // TODO: Use the connection string to configure the database context if needed. Otherwise, it can be removed.

        services.AddScoped<IGreetingsRepository, GreetingsRepository>();

        return services;
    }
}
