using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using Asp.Versioning;

using FastEndpoints;
using FastEndpoints.AspVersioning;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace StackedDeck.WebAPI.Template.API.Endpoints.Greetings;

/// <summary>
/// Endpoint for retrieving a greeting message using FastEndpoints.
/// </summary>
public class GreetingsEndpoint : EndpointWithoutRequest<GreetingsResponse>
{
    /// <summary>
    /// Configures the endpoint route and metadata.
    /// </summary>
    public override void Configure()
    {
        Get("/v{version:apiVersion}/greetings");
        Options(b => b
            .WithVersionSet(Constants.Api.Routes.Versioning.V1)
            .MapToApiVersion(new ApiVersion(1)));

        Description(d => d
                .WithSummary("Greets you.")
                .WithDescription("This is the default action, set up by the StackedDeck Web API project template using FastEndpoints with REPR pattern.")
                .Produces<GreetingsResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json));

        AllowAnonymous();
    }

    /// <summary>
    /// Handles the request and returns the response.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The response DTO.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new GreetingsResponse
        {
            Message = "Buongiorno!"
        };

        await Send.OkAsync(response, ct);
    }
}
