using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using FastEndpoints;

using Microsoft.AspNetCore.Http;

namespace StackedDeck.WebAPI.Template.API.Endpoints.Greetings;

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
        Summary(s =>
        {
            s.Summary = "Greets you.";
            s.Description = "This is the default action, set up by the StackedDeck Web API project template using FastEndpoints with REPR pattern.";
        });
        Description(d => d
            .Produces<GreetingsResponse>(200, MediaTypeNames.Application.Json));
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
