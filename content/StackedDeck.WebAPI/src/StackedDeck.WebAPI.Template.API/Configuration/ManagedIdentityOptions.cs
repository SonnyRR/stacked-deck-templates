namespace StackedDeck.WebAPI.Template.API.Configuration;

/// <summary>
/// Options related to Azure Managed Identity.
/// </summary>
public class ManagedIdentityOptions
{
    /// <summary>
    /// The configuration section name for Managed Identity options.
    /// </summary>
    public const string CFG_SECTION_NAME = "ManagedIdentity";

    /// <summary>
    /// The endpoint of the Azure App Configuration resource.
    /// </summary>
    public string AppConfigEndpoint { get; set; }
}
