namespace StackedDeck.WebAPI.Template.Build;

/// <summary>
/// The host environment for the application.
/// </summary>
public enum HostEnvironment
{
    /// <summary>
    /// The local development environment.
    /// </summary>
    Local,

    /// <summary>
    /// The non-production development environment.
    /// </summary>
    Development,

    /// <summary>
    /// The Staging environment.
    /// </summary>
    Staging,

    /// <summary>
    /// The Production environment.
    /// </summary>
    Production,

    /// <summary>
    /// The E2E testing environment.
    /// </summary>
    E2E
}
