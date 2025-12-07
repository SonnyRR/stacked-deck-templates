using System.ComponentModel.DataAnnotations;

namespace StackedDeck.WebAPI.Template.API.Configuration;

/// <summary>
/// Contains various connection strings for application dependencies.
/// </summary>
/// <remarks>
/// Extend this structure with additional connection strings as needed.
/// </remarks>
public class ConnectionStrings
{
    /// <summary>
    /// Represents the configuration section name used for connection strings.
    /// </summary>
    public const string CFG_SECTION_NAME = nameof(ConnectionStrings);

    /// <summary>
    /// The database connection string.
    /// </summary>
    /// <remarks>
    /// This is a placeholder property, if your application does not make use
    /// of a database, you can remove this property.
    /// </remarks>
    [Required]
    public string Database { get; set; }
}
