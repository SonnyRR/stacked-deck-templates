using System;

namespace StackedDeck.WebAPI.Template.API.Configuration;

/// <summary>
/// Options related to Azure Managed Identity.
/// </summary>
public class AzureAppConfigurationOptions
{
    /// <summary>
    /// The configuration section name for Managed Identity options.
    /// </summary>
    public const string CFG_SECTION_NAME = "AzureAppConfiguration";

    /// <summary>
    /// The endpoint of the Azure App Configuration resource.
    /// </summary>
    public Uri Endpoint { get; set; }
}
