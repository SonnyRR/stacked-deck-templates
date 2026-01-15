#if (UseFastEndpoints)
using System.Threading;
using System.Threading.Tasks;

using FastEndpoints;

namespace StackedDeck.WebAPI.Template.API.Endpoints;

/// <summary>
/// Endpoint for retrieving a greeting message using FastEndpoints.
/// </summary>
public class GreetingsEndpoint : Endpoint<GreetingsRequest, GreetingsResponse>
{
    /// <summary>
    /// Configures the endpoint route and metadata.
    /// </summary>
    public override void Configure()
    {
        Get("sd-api-route-prefix/v1/greetings");
        Summary(s => s.Summary = "Greets you.");
        Description(d => d
            .WithDescription("This is the default action, set up by the StackedDeck Web API project template using FastEndpoints with REPR pattern.")
            .Produces<GreetingsResponse>(200, "application/json"));
    }

    /// <summary>
    /// Handles the request and returns the response.
    /// </summary>
    /// <param name="req">The request DTO.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The response DTO.</returns>
    public override async Task HandleAsync(GreetingsRequest req, CancellationToken ct)
    {
        var response = new GreetingsResponse
        {
            Message = "Buongiorno!"
        };

        await SendAsync(response, cancellation: ct);
    }
}
#endif
