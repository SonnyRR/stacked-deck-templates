using System.ComponentModel;

using Spectre.Console.Cli;

namespace StackedDeck.CLI.Template.Commands.Settings;

/// <summary>
/// Command settings for the greet operation.
/// </summary>
public class GreetCommandSettings : CommandSettings
{
    /// <summary>
    /// Gets or sets the name to greet.
    /// </summary>
    [CommandOption("-n|--name")]
    [Description("The name of the person or entity to greet.")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets whether to use verbose output.
    /// </summary>
    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output.")]
    public bool Verbose { get; set; }
}
