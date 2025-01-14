using System;

using StackedDeck.WebAPI.Template.Build;

namespace Extensions;

/// <summary>
/// Extension methods for the <see cref="HostEnvironment"/> enum.
/// </summary>
public static class HostEnvironmentExtensions
{
    /// <summary>
    /// Checks if the current host environment is the E2E testing environment.
    /// </summary>
    /// <param name="env">The host environment.</param>
    /// <returns><see langword="true" /> if the host environment is 'E2E', otherwise - <see langword="false" />.</returns>
    public static bool IsE2EEnvironment(this HostEnvironment env) => env.IsEnvironment(nameof(HostEnvironment.E2E));

    /// <summary>
    /// Checks if the current host environment is the local development environment.
    /// </summary>
    /// <param name="env">The host environment.</param>
    /// <returns><see langword="true" /> if the host environment is 'Local', otherwise - <see langword="false" />.</returns>
    public static bool IsLocalEnvironment(this HostEnvironment env) => env.IsEnvironment(nameof(HostEnvironment.Local));

    /// <summary>
    /// Helper method for matching string values to <see cref="HostEnvironment"/> members.
    /// </summary>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="environment">
    /// The string value representation of the environment, that should be checked against the
    /// <paramref name="hostEnvironment"/>
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the host environment matches the value of <paramref name="environment"/>,
    /// otherwise - <see langword="false" />.
    /// </returns>
    private static bool IsEnvironment(this HostEnvironment hostEnvironment, string environment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(environment);

        return string.Equals(
            hostEnvironment.ToString(),
            environment,
            StringComparison.OrdinalIgnoreCase);
    }
}
