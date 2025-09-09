using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StackedDeck.WebAPI.Template.API.Controllers;

/// <summary>
/// The default template sample controller.
/// </summary>
public class GreetingsController : BaseApiController
{
    /// <summary>
    /// The default template sample action.
    /// </summary>
    [HttpGet]
    [EndpointSummary("Greets you.")]
    [EndpointDescription("This is the default action, set up by the StackedDeck Web API project template.")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public IActionResult Greet() => Ok("Buongiorno!");
}
