namespace StackedDeck.WebAPI.Template.Data.Interfaces;

/// <summary>
/// Defines a contract for retrieving greeting messages.
/// </summary>
public interface IGreetingsRepository
{
    /// <summary>
    /// Retrieves a greeting message.
    /// </summary>
    /// <returns>A string containing the greeting message.</returns>
    string GetGreeting();
}
