using Microsoft.AspNetCore.Mvc;

using StackedDeck.WebAPI.Template.API.Attributes;

namespace StackedDeck.WebAPI.Template.API.Controllers;

/// <summary>
/// The preconfigured base controller for the V1 of this API.
/// </summary>
[ApiController]
[Route("sd-api-route-prefix/v{version:apiVersion}/[controller]")]
[InheritableApiVersion(1)]
public class BaseApiController : ControllerBase;
