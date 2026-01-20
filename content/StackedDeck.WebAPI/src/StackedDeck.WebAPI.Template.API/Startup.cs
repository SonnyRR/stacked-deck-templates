using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

#if (UseFastEndpoints)
using FastEndpoints;
#endif

using Serilog;

using StackedDeck.WebAPI.Template.API.Configuration;
using StackedDeck.WebAPI.Template.API.Extensions;
using StackedDeck.WebAPI.Template.API.Middlewares;
using StackedDeck.WebAPI.Template.Common.Extensions;

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

        if (env.IsLocal() || env.IsE2E())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler();
        }

        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseCors();
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            Secure = CookieSecurePolicy.Always,
            HttpOnly = HttpOnlyPolicy.Always
        });
        app.UseRouting();
        app.UseStatusCodePages();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthCheckEndpoints(env);
            endpoints.MapOpenApiEndpoints(env, apiOptions);
#if (UseMinimalApis)
            endpoints.MapMinimalApiEndpoints();
#endif
#if (UseFastEndpoints)
            endpoints.MapFastEndpoints();
#endif
#if (UseControlelrs)
            endpoints.MapDefaultControllerRoute();
#endif
        });
    }

    /// <summary>
    /// Registers services and dependencies in the application's DI container.
    /// </summary>
    /// <param name="services">The DI container in which to register the services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddApiServices(Configuration, Environment, out var _);
    }
}
