#if (UseFastEndpoints)
namespace StackedDeck.WebAPI.Template.API.Endpoints;

/// <summary>
/// Response DTO for the Greetings endpoint.
/// </summary>
public class GreetingsResponse
{
    /// <summary>
    /// The greeting message.
    /// </summary>
    public string Message { get; set; } = default!;
}
#endif
