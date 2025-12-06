using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace StackedDeck.WebAPI.Template.API.Health;

/// <summary>
/// Provides a health check that verifies the application is alive and reports system information.
/// </summary>
public class LivenessHealthCheck : IHealthCheck
{
    /// <inheritdoc/>
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var metadata = new Dictionary<string, object>()
            {
                { "dotnetVersion", Environment.Version.ToString() },
                { "processors", Environment.ProcessorCount },
                { "osVersion", Environment.OSVersion.VersionString },
                { "os", Environment.OSVersion.Platform.ToString() }
            };

            return Task.FromResult(HealthCheckResult.Healthy("Server is live.", data: metadata));
        }
        catch (Exception ex)
        {
            var failure = HealthCheckResult.Unhealthy(
                "Server is not reporting base liveness metrics.",
                exception: ex);

            return Task.FromResult(failure);
        }
    }
}
