using System.Net;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using StackedDeck.WebAPI.Template.Integration.Tests.Fixtures;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Configuration;

/// <summary>
/// Contains integration tests that verify configuration values from Azure App Configuration
/// are correctly resolved in the API's IConfiguration.
/// </summary>
public class ConfigurationTests
{
    private readonly AzureAppConfigurationFixture azureAppConfigFixture;
    private readonly ApiFixture apiFixture;

    /// <summary>
    /// Creates a new instance of <see cref="ConfigurationTests"/>.
    /// </summary>
    public ConfigurationTests(AzureAppConfigurationFixture azureAppConfigFixture, ApiFixture apiFixture)
    {
        this.azureAppConfigFixture = azureAppConfigFixture;
        this.apiFixture = apiFixture;
    }

    /// <summary>
    /// Tests that configuration values from Azure App Configuration are resolved.
    /// </summary>
    [Fact(DisplayName = "Resolves value from App Configuration")]
    public void Configuration_ResolvesValueFromAppConfiguration_ReturnsSeededValue()
    {
        var configuration = apiFixture.ServiceScope.ServiceProvider.GetRequiredService<IConfiguration>();
        configuration["TestStringKey"].ShouldBe("TestStringValue");
    }

    /// <summary>
    /// Tests that configuration values are updated dynamically after an HTTP request triggers refresh.
    /// </summary>
    [Fact(DisplayName = "Updates dynamically after an HTTP request")]
    public async Task Configuration_UpdatesDynamicallyViaHttpRequest_ReturnsUpdatedValue()
    {
        var configurationBefore = apiFixture.ServiceScope.ServiceProvider.GetRequiredService<IConfiguration>();
        configurationBefore["DynamicKey"].ShouldBe("InitialValue");

        await azureAppConfigFixture.SetConfigurationSettingAsync(
            "DynamicKey",
            "UpdatedValue",
            cancellationToken: TestContext.Current.CancellationToken);

        await Task.Delay(2_000, TestContext.Current.CancellationToken);
        var response = await apiFixture.Client.GetAsync("/fast-way/health", TestContext.Current.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var newScope = apiFixture.ApiFactory.Services.CreateScope();
        var configurationAfter = newScope.ServiceProvider.GetRequiredService<IConfiguration>();
        configurationAfter["DynamicKey"].ShouldBe("UpdatedValue");
    }
}
