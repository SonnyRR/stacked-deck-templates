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
    /// <summary>
    /// Performs the health check asynchronously.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous health check operation.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
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

            return Task.FromResult(HealthCheckResult.Healthy("Server is ready", data: metadata));
        }
        catch (Exception ex)
        {
            var failure = HealthCheckResult.Unhealthy("Check failure", exception: ex);
            return Task.FromResult(failure);
        }
    }
}
