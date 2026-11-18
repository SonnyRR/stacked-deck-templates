using System;
#if (UseOTELCollector)
using System.Diagnostics;
#endif

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
#if (UseOTELCollector)
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
#endif

using StackedDeck.WebAPI.Template.Common;
#if (UseOTELCollector)
using OpenTelemetry.Trace;

using StackedDeck.WebAPI.Template.Integration.Tests.Utils;
#endif

namespace StackedDeck.WebAPI.Template.Integration.Tests.Factories;

/// <summary>
/// A generic API host factory, utilizing the E2E environment.
/// </summary>
public class ApiFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
#if (UseOTELCollector)
    /// <summary>
    /// The spans exported by the SUT, appended as soon as a sampled span ends.
    /// </summary>
    public ConcurrentItemCollection<Activity> ExportedActivities { get; } = [];

#endif
    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null, EnvironmentVariableTarget.Process);

        builder.UseEnvironment(Environments.E2E);
#if (UseOTELCollector)

        // The E2E environment points the OTLP exporters at a locally running collector, which is not available
        // under test. Attach an in-memory one to the same tracer provider, so the telemetry tests assert on exactly
        // what the OTLP exporter would push to the collector, without a collector or the network.
        builder.ConfigureTestServices(services =>
            services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddInMemoryExporter(ExportedActivities)));

#endif
        base.ConfigureWebHost(builder);
    }
}
