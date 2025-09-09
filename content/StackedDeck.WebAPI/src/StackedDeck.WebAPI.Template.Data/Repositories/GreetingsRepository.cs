using StackedDeck.WebAPI.Template.Common.Entities;

using StackedDeck.WebAPI.Template.Data.Interfaces;

namespace StackedDeck.WebAPI.Template.Data.Repositories;

/// <summary>
/// Provides an implementation of <see cref="IGreetingsRepository"/>
/// that returns a predefined greeting message.
/// </summary>
public class GreetingsRepository : IGreetingsRepository
{
    /// <summary>
    /// Retrieves a predefined greeting message.
    /// </summary>
    /// <returns>A string containing the default greeting message.</returns>
    public string GetGreeting() => new Greeting().Text;
}
