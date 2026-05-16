using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using Asp.Versioning;
#if (UseFastEndpoints)

using FastEndpoints;
using FastEndpoints.AspVersioning;
#endif

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
#if (UseOTELCollector)
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
#endif

using StackedDeck.WebAPI.Template.API.Configuration;
using StackedDeck.WebAPI.Template.API.Handlers;
using StackedDeck.WebAPI.Template.API.Health;
#if (UseAzureCloudProvider)

using StackedDeck.WebAPI.Template.Common.Extensions;
#endif
#if (UseFastEndpoints)

using static StackedDeck.WebAPI.Template.API.Constants;
#endif

using IPNetwork = System.Net.IPNetwork;

namespace StackedDeck.WebAPI.Template.API.Extensions;

/// <summary>
/// Extension methods for registering API related services in the main DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers API related services to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The web host configuration.</param>
    /// <param name="environment">The web host environment.</param>
    /// <param name="connectionStringsOptions">Exposes the connection strings of the application.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with all API related options configured.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        out IOptions<ConnectionStrings> connectionStringsOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

#if (UseAzureCloudProvider)
        services.AddApiConfigurationOptions(configuration, environment);
#else
        services.AddApiConfigurationOptions(configuration);
#endif

        // Build a temporary service provider to resolve strongly typed options,
        // that are required for other service registrations.
        using var sp = services.BuildServiceProvider();
        var apiOptions = sp.GetRequiredService<IOptions<ApiOptions>>().Value;

        services
            .AddHttpContextAccessor()
            .AddProblemDetails(options => options
                    .CustomizeProblemDetails = context => context
                        .ProblemDetails.WithHttpContextMetadata(context.HttpContext))
            .AddExceptionHandler<GlobalExceptionHandler>()
#if (UseFastEndpoints)
            .AddFastEndpoints()
#endif
#if (UseControllers)
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
#else
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
#endif

        services
           .AddCors(options => options
               .AddDefaultPolicy(policyBuilder =>
               {
                   policyBuilder
                       .SetIsOriginAllowedToAllowWildcardSubdomains()
                       .WithOrigins(apiOptions.CorsOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader();

                   // Set AllowCredentials() if you change the default origins and you plan to use SSO.
               }));

        services.AddApiVersioning();
        services.AddOpenApiSpecification();
        services.AddHealthProbes();
#if (UseOTELCollector)
        var otelOptions = sp.GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;
        services.AddTelemetry(apiOptions.Identifier, otelOptions);
#else
        services.AddTelemetry(apiOptions.Identifier);
#endif

        connectionStringsOptions = sp.GetRequiredService<IOptions<ConnectionStrings>>();

        return services;
    }

#if (UseAzureCloudProvider)
    /// <summary>
    /// Registers API specific configuration options in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The web host configuration.</param>
    /// <param name="environment">The web host environment.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with strongly typed API options configured.</returns>
    /// <exception cref="ArgumentNullException"/>
    private static IServiceCollection AddApiConfigurationOptions(
        this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
#else
    /// <summary>
    /// Registers API specific configuration options in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The web host configuration.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with strongly typed API options configured.</returns>
    /// <exception cref="ArgumentNullException"/>
    private static IServiceCollection AddApiConfigurationOptions(
        this IServiceCollection services, IConfiguration configuration)
#endif
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // NOTE: The strongly typed configuration options below are bound from the 'IConfiguration' instance,
        // which supports multiple providers such as: 'appsettings.{env}.json', 'Azure App Configuration',
        // environment variables, user secrets, etc. The configuration keys are expected to follow the patterns
        // mentioned in the example below, where the delimiter is the ':' character. This is mandatory to support
        // backwards compatibility between all configuration providers. It also reduces the amount of boilerplate
        // code for registering the options, since the registration statements & validations will be the same
        // for all environments.
        //
        // Example:
        //
        // appsettings.json -> ConnectionStrings:Database
        // Azure App Configuration -> {ApiIdentifier}:ConnectionStrings:Database
        //
        // It's important to know that there is also precedence between the configuration providers.
        // For instance, if you have a key 'ConnectionStrings:Database' defined both in 'appsettings.json' and
        // in Azure App Configuration, the value from 'Azure App Configuration' will take precedence and be the one
        // that gets bound to the strongly typed options. This allows you to override configuration values in
        // different environments without changing the underlying code or configuration files.
        // It also acts as a fallback mechanism, in case some configuration values are missing from one provider,
        // they can be supplied by another provider that has them defined.

        var apiOptionsSection = configuration.GetSection(ApiOptions.CFG_SECTION_NAME);
        services.Configure<ApiOptions>(apiOptionsSection)
            .AddOptionsWithValidateOnStart<ApiOptions>()
            .ValidateDataAnnotations();

#if (UseAzureCloudProvider)
        services.Configure<AzureAppConfigurationOptions>(configuration.GetSection(ManagedIdentityOptions.CFG_SECTION_NAME))
            .AddOptionsWithValidateOnStart<AzureAppConfigurationOptions>()
            .ValidateDataAnnotations()
            .Validate(options => environment.IsLocal() || (!string.IsNullOrWhiteSpace(options.AppConfigEndpoint) &&
                     Uri.IsWellFormedUriString(options.AppConfigEndpoint, UriKind.Absolute)));
#endif

        services.Configure<ConnectionStrings>(configuration.GetSection(ConnectionStrings.CFG_SECTION_NAME))
            .AddOptionsWithValidateOnStart<ConnectionStrings>()
            .ValidateDataAnnotations();

#if (UseOTELCollector)
        services.Configure<OpenTelemetryOptions>(configuration.GetSection(OpenTelemetryOptions.CFG_SECTION_NAME))
            .AddOptionsWithValidateOnStart<OpenTelemetryOptions>()
            .ValidateDataAnnotations();

#endif
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;

            // If your API will sit behind a reverse proxy, you will need to configure a list
            // of known networks in order for the 'ForwardedHeadersMiddleware' to correctly
            // forward all 'X-Forwarded-*' headers. By default these headers will not be forwarded
            // from untrusted networks. You must use CIDR ranges.
            foreach (var knownNetwork in apiOptionsSection.Get<ApiOptions>()?.KnownNetworks)
            {
                options.KnownIPNetworks.Add(IPNetwork.Parse(knownNetwork));
            }
        });

        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        return services;
    }

    /// <summary>
    /// Registers the API Versioning services in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with API Versioning services registered.</returns>
    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

#if (!UseFastEndpoints)
        services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = ApiVersion.Default;
                options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
#else
        services
            .AddVersioning(versioningOptions =>
            {
                versioningOptions.ReportApiVersions = true;
                versioningOptions.AssumeDefaultVersionWhenUnspecified = true;
                versioningOptions.DefaultApiVersion = ApiVersion.Default;
                versioningOptions.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
            }, explorerOptions =>
            {
                explorerOptions.GroupNameFormat = "'v'VVV";
                explorerOptions.SubstituteApiVersionInUrl = true;
            });

        VersionSets.CreateApi(Api.Routes.Versioning.V1_SET, v => v.HasApiVersion(new ApiVersion(1.0)));
#endif

        return services;
    }

    /// <summary>
    /// Registers the OpenAPI specification related services to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with OpenAPI specification services registered.</returns>
    private static IServiceCollection AddOpenApiSpecification(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

#pragma warning disable S125
        /*
         By default, this invocation will register the OpenAPI specification document
         as 'v1.json'. If you need to support multiple versions of this API, you'll have
         to explicitly register additional documents similar to:

         Examples:
            services.AddOpenApi();
            services.AddOpenApi("v2");
        */
#pragma warning restore S125

        services.AddOpenApi();

        return services;
    }

    /// <summary>
    /// Registers custom API health-check probes in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with health probes registered.</returns>
    private static IServiceCollection AddHealthProbes(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register additional infrastructure checks here.
        services
            .AddHealthChecks()
            .AddCheck<LivenessHealthCheck>("general", tags: ["all", "server"]);

        return services;
    }

#if (UseOTELCollector)
    /// <summary>
    /// Registers OpenTelemetry services in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="otelOptions">The OpenTelemetry configuration options.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with OpenTelemetry services registered.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    private static IServiceCollection AddTelemetry(this IServiceCollection services, string serviceName, OpenTelemetryOptions otelOptions)
#else
    /// <summary>
    /// Registers OpenTelemetry services in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with OpenTelemetry services registered.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    private static IServiceCollection AddTelemetry(this IServiceCollection services, string serviceName)
#endif
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

#if (UseOTELCollector)
        ArgumentNullException.ThrowIfNull(otelOptions);

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithMetrics(metrics => metrics
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otelOptions.Endpoint);
                    options.Protocol = otelOptions.Protocol;
                    if (otelOptions.Headers.Count > 0)
                    {
                        options.Headers = string.Join(",", otelOptions.Headers.Select(h => $"{h.Key}={h.Value}"));
                    }
                }))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otelOptions.Endpoint);
                    options.Protocol = otelOptions.Protocol;
                    if (otelOptions.Headers.Count > 0)
                    {
                        options.Headers = string.Join(",", otelOptions.Headers.Select(h => $"{h.Key}={h.Value}"));
                    }
                }));
#else
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithMetrics(metrics => metrics
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddPrometheusExporter());
#endif

        return services;
    }
}
