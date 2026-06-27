using Serilog;

namespace StackedDeck.CLI.Template;

/// <summary>
/// Entry point for the Stacked Deck CLI tool.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>The exit code.</returns>
    public static int Main(string[] args)
    {
        try
        {
            return CommandAppBuilder
                .Create()
                .Build()
                .Run(args);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
