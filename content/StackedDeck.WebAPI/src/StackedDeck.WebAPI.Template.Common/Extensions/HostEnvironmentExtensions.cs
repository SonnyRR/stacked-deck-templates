using System;

using Microsoft.Extensions.Hosting;

namespace StackedDeck.WebAPI.Template.Common.Extensions;

/// <summary>
/// Extension methods that add additional environment support for <see cref="IHostEnvironment"/>.
/// </summary>
public static class HostEnvironmentExtensions
{
    /// <summary>
    /// Checks if the current host environment name is <see cref="Environments.E2E"/>.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/>.</param>
    /// <returns><see langword="true"/> if the environment name is E2E, otherwise <see langword="false"/>.</returns>
    public static bool IsE2E(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        return hostEnvironment.IsEnvironment(Environments.E2E);
    }

    /// <summary>
    /// Checks if the current host environment name is <see cref="Environments.Local"/>.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/>.</param>
    /// <returns><see langword="true"/> if the environment name is Local, otherwise <see langword="false"/>.</returns>
    public static bool IsLocal(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        return hostEnvironment.IsEnvironment(Environments.Local);
    }
}
