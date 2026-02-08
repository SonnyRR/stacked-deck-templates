using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using OpenTelemetry.Exporter;

namespace StackedDeck.WebAPI.Template.API.Configuration;

/// <summary>
/// Configuration options for OpenTelemetry OTLP export.
/// </summary>
public sealed class OpenTelemetryOptions
{
    /// <summary>
    /// The configuration section name used for OpenTelemetry-related settings.
    /// </summary>
    public const string CFG_SECTION_NAME = "OpenTelemetry";

    /// <summary>
    /// The OTLP collector endpoint URL.
    /// </summary>
    /// <remarks>
    /// Default value is "http://localhost:4317" for local development.
    /// </remarks>
    [Required]
    [Url]
    public string Endpoint { get; set; }

    /// <summary>
    /// The export protocol.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="OtlpExportProtocol.Grpc"/>.
    /// </remarks>
    public OtlpExportProtocol Protocol { get; set; }

    /// <summary>
    /// Optional headers for authentication.
    /// </summary>
    /// <remarks>
    /// Can be used to pass API keys or other authentication headers to the collector.
    /// </remarks>
    public Dictionary<string, string> Headers { get; set; } = [];
}
