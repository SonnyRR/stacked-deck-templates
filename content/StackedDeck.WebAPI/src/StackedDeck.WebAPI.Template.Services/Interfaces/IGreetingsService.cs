namespace StackedDeck.WebAPI.Template.Services.Interfaces;

/// <summary>
/// Defines a contract for a greeting service that provides greeting messages.
/// </summary>
public interface IGreetingsService
{
    /// <summary>
    /// Retreives and returns a greeting message.
    /// </summary>
    /// <returns>A string containing the greeting message.</returns>
    string Greet();
}
