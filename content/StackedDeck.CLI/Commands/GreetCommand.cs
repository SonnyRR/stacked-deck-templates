using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Serilog;

using Spectre.Console;
using Spectre.Console.Cli;

using StackedDeck.CLI.Template.Commands.Settings;

namespace StackedDeck.CLI.Template.Commands;

/// <summary>
/// A sample greeting command for the CLI application.
/// </summary>
public class GreetCommand : AsyncCommand<GreetCommandSettings>
{
    private readonly IConfigurationRoot configuration;
    private readonly IAnsiConsole console;

    /// <summary>
    /// Initializes a new instance of the <see cref="GreetCommand"/> class.
    /// </summary>
    /// <param name="configuration">The configuration root.</param>
    /// <param name="console">The Ansi console for output.</param>
    public GreetCommand(IConfigurationRoot configuration, IAnsiConsole console)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc/>
    protected override async Task<int> ExecuteAsync(CommandContext context, GreetCommandSettings settings, CancellationToken cancellationToken)
    {
        try
        {
            var title = configuration["Title"] ?? "StackedDeck CLI";
            var name = !string.IsNullOrWhiteSpace(settings.Name) ? settings.Name : "World";

            var headerPanel = new Panel(
                new Markup($"{title}\n[grey]Author: templateAuthor | {DateTime.Now:dd/MM/yyyy}[/]"))
                .Header("[bold cyan]StackedDeck CLI[/]")
                .BorderColor(Color.Cyan);

            console.Write(headerPanel);

            if (settings.Verbose)
            {
                console.Write(new Rule("[yellow]Verbose mode enabled[/]").RuleStyle("yellow").Centered());
                console.MarkupLine($"[grey]Environment: {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}[/]");
            }

            console.MarkupLine($"[green]Hello, {name}![/]");

            Log.Information("Greeted {Name} successfully", name);

            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception");
            console.WriteException(ex);

            return 1;
        }
    }
}
