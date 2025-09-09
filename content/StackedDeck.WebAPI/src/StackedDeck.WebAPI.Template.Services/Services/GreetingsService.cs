using StackedDeck.WebAPI.Template.Data.Interfaces;

using StackedDeck.WebAPI.Template.Services.Interfaces;

namespace StackedDeck.WebAPI.Template.Services.Services;

/// <summary>
/// Provides a greeting service that retrieves greeting messages from a repository.
/// </summary>
public class GreetingsService : IGreetingsService
{
    private readonly IGreetingsRepository greetingsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GreetingsService"/> class.
    /// </summary>
    /// <param name="greetingsRepository">The repository used to retrieve greeting messages.</param>
    public GreetingsService(IGreetingsRepository greetingsRepository) => this.greetingsRepository = greetingsRepository;

    /// <summary>
    /// Retrieves a greeting message from the repository.
    /// </summary>
    /// <returns>A string containing the greeting message.</returns>
    public string Greet() => greetingsRepository.GetGreeting();
}
