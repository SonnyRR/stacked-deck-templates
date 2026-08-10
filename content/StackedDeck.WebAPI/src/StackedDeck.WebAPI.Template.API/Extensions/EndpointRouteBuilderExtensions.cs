using System;
using System.Net.Mime;
using System.Text.Json;
#if (UseMinimalApis)

using Asp.Versioning;
#endif

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Scalar.AspNetCore;

using StackedDeck.WebAPI.Template.API.Configuration;

using static StackedDeck.WebAPI.Template.API.Constants;

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
        builder.MapHealthChecks($"{Api.Routes.PREFIX}/health", healthCheckOptions);

        return builder;
    }

    /// <summary>
    /// Maps OpenAPI endpoints for the API, including Scalar.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="environment">The web host environment.</param>
    /// <param name="apiOptions">The API options.</param>
    /// <returns>The endpoint route builder with OpenAPI endpoints mapped.</returns>
    public static IEndpointRouteBuilder MapOpenApiEndpoints(
        this IEndpointRouteBuilder builder, IWebHostEnvironment environment, IOptions<ApiOptions> apiOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (!environment.IsProduction())
        {
            builder
                .MapOpenApi($"{Api.Routes.PREFIX}/openapi/{{documentName}}.json")
                .WithDocumentPerVersion();

            builder.MapScalarApiReference(
                $"{Api.Routes.PREFIX}/documentation",
                options =>
                {
                    options.WithOperationTitleSource(OperationTitleSource.Path);
                    options.SortTagsAlphabetically();

                    var descriptions = builder.DescribeApiVersions();

                    for (var i = 0; i < descriptions.Count; i++)
                    {
                        var description = descriptions[i];
                        var isDefault = i == descriptions.Count - 1;

                        options.AddDocument(
                                documentName: description.GroupName,
                                title: description.GroupName,
                                routePattern: $"{Api.Routes.PREFIX}/openapi/v{description.ApiVersion.MajorVersion}.json",
                                isDefault: isDefault);
                    }
                });
        }

        return builder;
    }
#if (UseMinimalApis)

    /// <summary>
    /// Maps minimal API endpoints for the API.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The endpoint route builder with minimal API endpoints mapped.</returns>
    public static IEndpointRouteBuilder MapMinimalApiEndpoints(this IEndpointRouteBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var api = builder.NewVersionedApi();

        var v1Group = api
            .MapGroup($"{Api.Routes.PREFIX}/v{{version:apiVersion}}")
            .HasApiVersion(new ApiVersion(1));

        v1Group
            .MapGet("/greetings", () => "Buongiorno!")
            .WithName("GetGreetings")
            .WithTags("Greetings")
            .WithSummary("Greets you.")
            .WithDescription("This is the default action, set up by the StackedDeck Web API project template using Minimal APIs.")
            .Produces<string>(StatusCodes.Status200OK);

        return builder;
    }
#endif
#if (UsePrometheusScrape)

    /// <summary>
    /// Maps the Prometheus metrics scraping endpoint.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The endpoint route builder with the metrics endpoint mapped.</returns>
    public static IEndpointRouteBuilder MapMetricsEndpoint(this IEndpointRouteBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.MapPrometheusScrapingEndpoint(Api.Routes.PREFIX + "/metrics");

        return builder;
    }
#endif
}
