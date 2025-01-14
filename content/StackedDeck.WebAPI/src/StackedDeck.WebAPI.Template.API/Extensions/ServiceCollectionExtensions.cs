using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Asp.Versioning;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using StackedDeck.WebAPI.Template.API.Configuration;
using StackedDeck.WebAPI.Template.API.Handlers;
using StackedDeck.WebAPI.Template.Common.Configuration;
using StackedDeck.WebAPI.Template.Common.Extensions;

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

        services.AddApiConfigurationOptions(configuration, environment);

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
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

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

        connectionStringsOptions = sp.GetRequiredService<IOptions<ConnectionStrings>>();

        return services;
    }

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
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var apiOptionsSection = configuration.GetSection(ApiOptions.CFG_SECTION_NAME);
        services.Configure<ApiOptions>(apiOptionsSection)
            .AddOptionsWithValidateOnStart<ApiOptions>()
            .ValidateDataAnnotations();

        services.Configure<ManagedIdentityOptions>(configuration.GetSection(ManagedIdentityOptions.CFG_SECTION_NAME))
            .AddOptionsWithValidateOnStart<ManagedIdentityOptions>()
            .ValidateDataAnnotations()
            .Validate(options => environment.IsLocal() || environment.IsE2E() ||
                    (!string.IsNullOrWhiteSpace(options.AppConfigEndpoint) &&
                     Uri.IsWellFormedUriString(options.AppConfigEndpoint, UriKind.Absolute)));

        // Change the options configuration root based on the environment.
        // Azure App Configuration keys will be prefixed with the API identifier,
        // which is part of the 'ApiOptions' configuration section. The reminder of the
        // configuration paths will follow the same convention as the ones in the
        // 'appsettings.json' documents.
        //
        // Example:
        //
        // appsettings.json -> ConnectionStrings:Database
        // Azure App Configuration -> {ApiIdentifier}:ConnectionStrings:Database
        //
        // This allows for backwards compatibility with the normal appsettings.json configuration
        // schema. Also reduces the amount of boilerplate code for registering the options, since
        // the registration statements & validations will be the same for all environments.
        var optionsConfigurationRoot = environment.IsLocal() || environment.IsE2E()
            ? configuration
            : configuration.GetSection(apiOptionsSection.GetSection(nameof(ApiOptions.Identifier)).Value);

        services.Configure<ConnectionStrings>(optionsConfigurationRoot.GetSection(ConnectionStrings.CFG_SECTION_NAME))
            .AddOptionsWithValidateOnStart<ConnectionStrings>()
            .ValidateDataAnnotations();

        services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.All);

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
}
