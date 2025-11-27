using System.ComponentModel.DataAnnotations;

namespace StackedDeck.WebAPI.Template.API.Configuration;

/// <summary>
/// General options, that relate to the API itself.
/// </summary>
public sealed class ApiOptions
{
    /// <summary>
    /// The configuration section name used for API-related settings.
    /// </summary>
    public const string CFG_SECTION_NAME = "API";

    /// <summary>
    /// The title of the API.
    /// </summary>
    /// <remarks>
    /// Primarily used in the OpenAPI specification.
    /// </remarks>
    [Required]
    public string Title { get; init; }

    /// <summary>
    /// The description of the API.
    /// </summary>
    /// <remarks>
    /// Primarily used in the OpenAPI specification.
    /// </remarks>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// The CORS origins that can interact with this API.
    /// </summary>
    [Required]
    public string[] CorsOrigins { get; set; }

    /// <summary>
    /// The unique identifier for the API.
    /// </summary>
    /// <remarks>
    /// Used for hooking up Azure App Configuration.
    /// </remarks>
    [Required]
    public string Identifier { get; set; }

    /// <summary>
    /// A collection of networks that are considered trusted.
    /// </summary>
    /// <remarks>
    /// Primarily consumed by the 'ForwardedHeadersMiddleware'
    /// to determine if the request's originating IP is from a
    /// known network. Because by default 'X-Forwarded-*' headers
    /// will not be forwarded from unknown networks.
    /// </remarks>
    public string[] KnownNetworks { get; set; }
}
