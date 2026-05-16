using System.Threading.Tasks;

using Azure.Data.AppConfiguration;

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
    private readonly IConfiguration configuration;

    private const string API_IDENTIFIER = "api-identifier";

    /// <summary>
    /// Creates a new instance of <see cref="ConfigurationTests"/>.
    /// </summary>
    public ConfigurationTests(AzureAppConfigurationFixture azureAppConfigFixture, ApiFixture apiFixture)
    {
        this.azureAppConfigFixture = azureAppConfigFixture;
        this.configuration = apiFixture.ServiceScope.ServiceProvider.GetRequiredService<IConfiguration>();
    }

    /// <summary>
    /// Tests that a simple string configuration key is resolved from App Configuration.
    /// </summary>
    [Fact(DisplayName = "Configuration resolves string value from App Configuration")]
    public async Task Configuration_ResolvesStringValue_FromAppConfiguration()
    {
        var testKey = $"{API_IDENTIFIER}:TestStringKey";
        var testValue = "TestStringValue";

        await azureAppConfigFixture.SeedAsync(testKey, testValue, cancellationToken: TestContext.Current.CancellationToken);

        configuration["TestStringKey"].ShouldBe(testValue);
    }

    /// <summary>
    /// Tests that a nested configuration key is resolved from App Configuration.
    /// </summary>
    [Fact(DisplayName = "Configuration resolves nested configuration key from App Configuration")]
    public async Task Configuration_ResolvesNestedKey_FromAppConfiguration()
    {
        var testKey = $"{API_IDENTIFIER}:Features:NestedSetting";
        var testValue = "NestedValue123";

        await azureAppConfigFixture.SeedAsync(testKey, testValue, cancellationToken: TestContext.Current.CancellationToken);

        configuration["Features:NestedSetting"].ShouldBe(testValue);
    }

    /// <summary>
    /// Tests that a boolean configuration value is resolved from App Configuration.
    /// </summary>
    [Fact(DisplayName = "Configuration resolves boolean value from App Configuration")]
    public async Task Configuration_ResolvesBooleanValue_FromAppConfiguration()
    {
        var testKey = $"{API_IDENTIFIER}:FeatureFlags:Enabled";
        var testValue = "true";

        await azureAppConfigFixture.SeedAsync(testKey, testValue, cancellationToken: TestContext.Current.CancellationToken);

        configuration["FeatureFlags:Enabled"].ShouldBe(testValue);
    }

    /// <summary>
    /// Tests that multiple configuration keys are resolved from App Configuration.
    /// </summary>
    [Fact(DisplayName = "Configuration resolves multiple keys from App Configuration")]
    public async Task Configuration_ResolvesMultipleKeys_FromAppConfiguration()
    {
        var settings = new[]
        {
            new ConfigurationSetting($"{API_IDENTIFIER}:MultiKey1", "Value1"),
            new ConfigurationSetting($"{API_IDENTIFIER}:MultiKey2", "Value2"),
            new ConfigurationSetting($"{API_IDENTIFIER}:MultiKey3", "Value3"),
        };

        await azureAppConfigFixture.SeedAsync(settings, TestContext.Current.CancellationToken);

        configuration["MultiKey1"].ShouldBe("Value1");
        configuration["MultiKey2"].ShouldBe("Value2");
        configuration["MultiKey3"].ShouldBe("Value3");
    }

    /// <summary>
    /// Tests that an integer configuration value is resolved from App Configuration.
    /// </summary>
    [Fact(DisplayName = "Configuration resolves integer value from App Configuration")]
    public async Task Configuration_ResolvesIntegerValue_FromAppConfiguration()
    {
        var testKey = $"{API_IDENTIFIER}:MaxRetries";
        var testValue = "5";

        await azureAppConfigFixture.SeedAsync(testKey, testValue, cancellationToken: TestContext.Current.CancellationToken);

        configuration["MaxRetries"].ShouldBe(testValue);
    }
}
