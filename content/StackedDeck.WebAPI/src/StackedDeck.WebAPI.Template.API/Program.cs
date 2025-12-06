using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Serilog;
#if (UseAzureCloudProvider)

using StackedDeck.WebAPI.Template.API.Extensions;
#endif

namespace StackedDeck.WebAPI.Template.API;

/// <summary>
/// The main program class, which contains the entry point for the API.
/// </summary>
[SuppressMessage(
    "Roslynator",
    "RCS1102:Make class static",
    Justification = "Class cannot be 'static' because it is used in the web host factory for integration tests.")]
[SuppressMessage(
    "Major Code Smell",
    "S1118:Utility classes should not have public constructors",
    Justification = "Class cannot be 'static' because it is used in the web host factory for integration tests.")]
public class Program
{
    /// <summary>
    /// The entrypoint for bootstrapping the API.
    /// </summary>
    /// <param name="args">Arguments for the host builder.</param>
    public static void Main(string[] args)
    {
        try
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .UseSerilog((builderContext, loggerConfig)
                    => loggerConfig.ReadFrom.Configuration(builderContext.Configuration))
#if (UseAzureCloudProvider)
                .AddAzureAppConfiguration()
#endif
                .Build()
                .Run();
        }
        catch (Exception ex)
        {
            if (Log.Logger is null || Log.Logger.GetType().Name == "SilentLogger")
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger();
            }

            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();
        }
    }
}
