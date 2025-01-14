using MicrosoftEnvironments = Microsoft.Extensions.Hosting.Environments;

namespace StackedDeck.WebAPI.Template.Common;

/// <summary>
/// Exposes constants for the different environments used in the application.
/// </summary>
public static class Environments
{
    /// <inheritdoc cref="MicrosoftEnvironments.Development"/>
    public static readonly string Development = MicrosoftEnvironments.Development;

    /// <summary>
    /// Specifies the E2E environment.
    /// </summary>
    /// <remarks>
    /// The End-to-End environment is intended for automation tests &amp; shouldn't be used for anything else.
    /// </remarks>
    public static readonly string E2E = "E2E";

    /// <summary>
    /// Specifies the Local environment.
    /// </summary>
    /// <remarks>
    /// Intended to be used for local development and testing.
    /// </remarks>
    public static readonly string Local = "Local";

    /// <inheritdoc cref="MicrosoftEnvironments.Production"/>
    public static readonly string Production = MicrosoftEnvironments.Production;

    /// <inheritdoc cref="MicrosoftEnvironments.Staging"/>
    public static readonly string Staging = MicrosoftEnvironments.Staging;
}
