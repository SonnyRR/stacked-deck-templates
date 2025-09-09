using System.ComponentModel;

using Nuke.Common.Tooling;

namespace StackedDeck.WebAPI.Template.Build;

/// <summary>
/// Represents the build configuration (Debug or Release) as an enumeration.
/// </summary>
[TypeConverter(typeof(TypeConverter<Configuration>))]
public class Configuration : Enumeration
{
    /// <summary>
    /// Represents the Debug build configuration.
    /// </summary>
    public static readonly Configuration Debug = new() { Value = nameof(Debug) };

    /// <summary>
    /// Represents the Release build configuration.
    /// </summary>
    public static readonly Configuration Release = new() { Value = nameof(Release) };

    /// <summary>
    /// Converts a <see cref="Configuration"/> instance to a string representation of its value.
    /// </summary>
    /// <param name="configuration">The configuration instance to convert.</param>
    /// <returns>The string value of the configuration (e.g., "Debug" or "Release").</returns>
    public static implicit operator string(Configuration configuration) => configuration.Value;
}
