using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Scalar.AspNetCore;

using Serilog;

using StackedDeck.WebAPI.Template.API.Configuration;
using StackedDeck.WebAPI.Template.API.Extensions;
using StackedDeck.WebAPI.Template.API.Middlewares;
using StackedDeck.WebAPI.Template.Common.Extensions;
using StackedDeck.WebAPI.Template.Data.Extensions;
using StackedDeck.WebAPI.Template.Services.Extensions;

namespace StackedDeck.WebAPI.Template.API;

/// <summary>
/// Containing the startup methods for the ASP.NET Core application
/// </summary>
public class Startup
{
    /// <summary>
    /// Creates a default instance of <see cref="Startup"/>.
    /// </summary>
    /// <param name="configuration">The application's configuration.</param>
    /// <param name="environment">The application's web host environment.</param>
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    /// <summary>
    /// The application's configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// The application's hosting environment.
    /// </summary>
    public IWebHostEnvironment Environment { get; }

    /// <summary>
    /// Configures the ASP.NET Core request pipeline.
    /// </summary>
    /// <remarks>
    /// Use it to configure your middlewares.
    /// </remarks>
    /// <param name="app">The application builder.</param>
    /// <param name="env">The host environment.</param>
    /// <param name="apiOptions">The general API options.</param>
    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ApiOptions> apiOptions)
    {
        app.UseSerilogRequestLogging();

        if (env.IsDevelopment() || env.IsLocal())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseCors();
        app.UseCookiePolicy(new CookiePolicyOptions { Secure = CookieSecurePolicy.Always });
        app.UseRouting();
        app.UseStatusCodePages();
        app.UseEndpoints(endpoints =>
        {
            if (!env.IsProduction())
            {
                endpoints.MapOpenApi();
                endpoints.MapScalarApiReference(
                    "/sd-api-route-prefix/documentation",
                    options =>
                    {
                        options.WithTitle(apiOptions.Value.Title);
                        options.WithClientButton();
                        options.WithOperationTitleSource(OperationTitleSource.Path);
                        options.WithModels();
                        options.WithDarkModeToggle();
                        options.WithTagSorter(TagSorter.Alpha);

#pragma warning disable S125
                        /*
                         If you decide to support multiple versions of this API,you'll need to specify
                         the OpenAPI spec documents explicitly. The route pattern is the default one,
                         set up by the 'endpoints.MapOpenApi()'.The document name (v1.json, v2.json) are
                         configured by the ServiceCollectionExtensions::AddOpenApiSpecification() extension method.

                         Examples:
                            options.AddDocument("v1", routePattern:"openapi/v1.json");
                            options.AddDocument("v2", routePattern:"openapi/v2.json");
                         */
#pragma warning restore S125
                    });
            }

            endpoints.MapDefaultControllerRoute();
        });
    }

    /// <summary>
    /// Registers services and dependencies in the application's DI container.
    /// </summary>
    /// <param name="services">The DI container in which to register the services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddApiServices(Configuration, Environment, out var connectionStringsOptions);
        services.AddPersistenceServices(connectionStringsOptions.Value.Database);
        services.AddCoreServices();
    }
}
