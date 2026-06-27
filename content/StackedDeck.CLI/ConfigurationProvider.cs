using System;

using Microsoft.Extensions.Configuration;

using Serilog;

namespace StackedDeck.CLI.Template;

/// <summary>
/// Provides configuration building and retrieval for the application.
/// </summary>
public static class ConfigurationProvider
{
    /// <summary>
    /// Builds the application configuration by loading JSON settings files.
    /// </summary>
    /// <param name="environment">The environment name. Defaults to DOTNET_ENVIRONMENT or Development.</param>
    /// <param name="basePath">The base path for configuration files.</param>
    /// <returns>The built configuration root.</returns>
    public static IConfigurationRoot BuildConfiguration(string environment = null, string basePath = null)
    {
        var builder = new ConfigurationBuilder();

        if (!string.IsNullOrEmpty(basePath))
        {
            builder.SetBasePath(basePath);
        }

        return builder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{environment ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json",
                optional: true,
                reloadOnChange: true)
            .Build();
    }

    /// <summary>
    /// Configures Serilog logging from the provided configuration.
    /// </summary>
    /// <param name="configuration">The configuration root to read logging settings from.</param>
    public static void ConfigureSerilog(IConfigurationRoot configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}
