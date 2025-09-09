using System;

using Microsoft.Extensions.DependencyInjection;

using StackedDeck.WebAPI.Template.Services.Interfaces;
using StackedDeck.WebAPI.Template.Services.Services;

namespace StackedDeck.WebAPI.Template.Services.Extensions;

/// <summary>
/// Common service collection extensions for registering Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers core application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IGreetingsService, GreetingsService>();

        return services;
    }
}
