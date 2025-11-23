using System;
using System.Net.Mime;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace StackedDeck.WebAPI.Template.API.Extensions;

/// <summary>
/// Extension methods for registering custom API routes.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps health check endpoints for the API.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="environment">The web host environment.</param>
    /// <returns>The endpoint route builder with health check endpoints mapped.</returns>
    public static IEndpointRouteBuilder MapHealthCheckEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(environment);

        var healthCheckOptions = new HealthCheckOptions
        {
            ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status418ImATeapot,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
        };

        // You shouldn't expose unnecessary information on public environments,
        // because it's a security concern. By default, on non-prod environments,
        // this will use a custom response writer, so that custom metadata is
        // serialized in the response. This can be useful for Developers, DevOps & QAs
        // to debug & confirm service health. If you wish to expose this information
        // on PROD environment, you'll have to authorize it behind custom roles/policies.
        if (!environment.IsProduction())
        {
            healthCheckOptions.ResponseWriter = (context, healthReport) =>
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var jsonDocument = JsonSerializer.Serialize(healthReport.Entries);

                return context.Response.WriteAsync(jsonDocument);
            };
        }

        // If you wish, you can split up your infrastructure checks
        // in separate endpoints, by leveraging tags. You'll have
        // to compose additional 'HealthCheckOptions' instances, so
        // that you override the default filter predicate to match
        // your custom tag(s).
        builder.MapHealthChecks("/sd-api-route-prefix/health", healthCheckOptions);

        return builder;
    }
}
