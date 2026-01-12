using System;
using System.Diagnostics.CodeAnalysis;
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

namespace StackedDeck.WebAPI.Template.API.Extensions;

/// <summary>
/// Extension methods for registering custom API routes.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    private const string API_ROUTE_PREFIX = "/sd-api-route-prefix";

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
        builder.MapHealthChecks($"{API_ROUTE_PREFIX}/health", healthCheckOptions);

        return builder;
    }

    /// <summary>
    /// Maps OpenAPI endpoints for the API, including Scalar.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="environment">The web host environment.</param>
    /// <param name="apiOptions">The API options.</param>
    /// <returns>The endpoint route builder with OpenAPI endpoints mapped.</returns>
    [SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "Documentation w/ code example.")]
    public static IEndpointRouteBuilder MapOpenApiEndpoints(
        this IEndpointRouteBuilder builder, IWebHostEnvironment environment, IOptions<ApiOptions> apiOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (!environment.IsProduction())
        {
            builder.MapOpenApi();
            builder.MapScalarApiReference(
                $"{API_ROUTE_PREFIX}/documentation",
                options =>
                {
                    options.WithTitle(apiOptions.Value.Title);
                    options.WithOperationTitleSource(OperationTitleSource.Path);
                    options.SortTagsAlphabetically();

                    // If you decide to support multiple versions of this API,you'll need to specify
                    // the OpenAPI spec documents explicitly. The route pattern is the default one,
                    // set up by the 'endpoints.MapOpenApi()'.The document name (v1.json, v2.json) are
                    // configured by the ServiceCollectionExtensions::AddOpenApiSpecification() extension method.
                    //
                    // Examples:
                    //    options.AddDocument("v1", routePattern:"openapi/v1.json");
                    //    options.AddDocument("v2", routePattern:"openapi/v2.json");
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

        var apiVersionSet = builder.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .Build();

        var v1Group = builder.MapGroup($"{API_ROUTE_PREFIX}/v{{version:apiVersion}}")
            .WithApiVersionSet(apiVersionSet);

        v1Group.MapGet("/greetings", () => Results.Ok("Buongiorno!"))
            .WithName("GetGreetings")
            .WithSummary("Greets you.")
            .WithDescription("This is the default action, set up by the StackedDeck Web API project template using Minimal APIs.")
            .Produces<string>(StatusCodes.Status200OK);

        return builder;
    }
#endif
}
