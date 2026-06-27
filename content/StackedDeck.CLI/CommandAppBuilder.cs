using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;
using Spectre.Console.Cli;

using StackedDeck.CLI.Template.Commands;
using StackedDeck.CLI.Template.DependencyInjection;

namespace StackedDeck.CLI.Template;

/// <summary>
/// Builds and configures the CLI command application.
/// </summary>
public class CommandAppBuilder
{
    private CommandApp commandApp;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAppBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <param name="console">The Ansi console.</param>
    internal CommandAppBuilder(
        IServiceCollection services,
        IConfigurationRoot configuration,
        IAnsiConsole console)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the configuration root.
    /// </summary>
    public IConfigurationRoot Configuration { get; }

    /// <summary>
    /// Gets the Ansi console.
    /// </summary>
    public IAnsiConsole Console { get; }

    private void ConfigureServices()
    {
        Services.AddSingleton(Configuration);
        Services.AddSingleton<IConfiguration>(Configuration);
        Services.AddSingleton(Console);
    }

    /// <summary>
    /// Builds the command application.
    /// </summary>
    /// <returns>The configured command app.</returns>
    public CommandApp Build()
    {
        if (commandApp != null)
        {
            return commandApp;
        }

        ConfigureServices();

        var registrar = new SpectreCliTypeRegistrar(Services);

        commandApp = new CommandApp(registrar);
        commandApp.Configure(config =>
        {
            config.SetApplicationName("sd-cli");
            config.AddExample("--name StackedDeck");
            config.AddCommand<GreetCommand>("greet");
        });

        return commandApp;
    }

    /// <summary>
    /// Creates a new command app builder with default settings.
    /// </summary>
    /// <param name="environment">The environment name.</param>
    /// <param name="console">
    /// The ANSI console instance. If not provided, defaults to <see cref="AnsiConsole.Console"/>.
    /// </param>
    /// <returns>A new <see cref="CommandAppBuilder"/> instance.</returns>
    public static CommandAppBuilder Create(string environment = null, IAnsiConsole console = null)
    {
        console ??= AnsiConsole.Console;

        var configuration = ConfigurationProvider.BuildConfiguration(environment);
        ConfigurationProvider.ConfigureSerilog(configuration);

        return new CommandAppBuilder(new ServiceCollection(), configuration, console);
    }
}
