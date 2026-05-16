using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.Data.AppConfiguration;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Fixtures;

/// <summary>
/// A global xUnit v3 fixture that provides an Azure App Configuration emulator container using Testcontainers.
/// </summary>
public sealed class AzureAppConfigurationFixture : IAsyncLifetime
{
    private const string IMAGE_NAME = "mcr.microsoft.com/azure-app-configuration/app-configuration-emulator:1.0.2";
    private const int CONTAINER_PORT = 8483;
    private const string ACCESS_KEY_ID = "fm-azcfg-emu";
    private const string ACCESS_KEY_SECRET = "abcdefghijklmnopqrstuvwxyz1234567890";

    /// <summary>
    /// Creates a new instance of <see cref="AzureAppConfigurationFixture"/>
    /// </summary>
    public AzureAppConfigurationFixture()
    {
        Container = new ContainerBuilder(IMAGE_NAME)
            .WithName($"azcfg-{Guid.NewGuid():N}")
            .WithPortBinding(CONTAINER_PORT, true)
            .WithEnvironment("Tenant:AnonymousAuthEnabled", "true")
            .WithEnvironment("Authentication:Anonymous:AnonymousUserRole", "Owner")
            .WithEnvironment("Tenant:HmacSha256Enabled", "true")
            .WithEnvironment("Tenant:AccessKeys:0:Id", ACCESS_KEY_ID)
            .WithEnvironment("Tenant:AccessKeys:0:Secret", ACCESS_KEY_SECRET)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(CONTAINER_PORT)))
            .Build();
    }

    /// <summary>
    /// The connection string for connecting to the Azure App Configuration emulator.
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// The endpoint URL of the running Azure App Configuration emulator.
    /// </summary>
    public string Endpoint { get; private set; } = string.Empty;

    /// <summary>
    /// The mapped host port of the Azure App Configuration emulator container.
    /// </summary>
    public int PortNumber { get; private set; }

    /// <summary>
    /// The Testcontainers container instance for the Azure App Configuration emulator.
    /// </summary>
    public IContainer Container { get; }

    /// <summary>
    /// The client for interacting with the Azure App Configuration emulator.
    /// </summary>
    public ConfigurationClient Client { get; private set; }

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        await Container.StartAsync();

        var mappedPort = Container.GetMappedPublicPort(CONTAINER_PORT);
        PortNumber = mappedPort;
        Endpoint = $"http://localhost:{mappedPort}";
        ConnectionString = $"Endpoint={Endpoint};Id={ACCESS_KEY_ID};Secret={ACCESS_KEY_SECRET}";

        Client = new ConfigurationClient(ConnectionString);
    }

    /// <summary>
    /// Seeds configuration settings into the emulator.
    /// </summary>
    /// <param name="settings">The configuration settings to seed.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task SeedAsync(IEnumerable<ConfigurationSetting> settings, CancellationToken cancellationToken = default)
    {
        foreach (var setting in settings)
        {
            await Client.AddConfigurationSettingAsync(setting, cancellationToken);
        }
    }

    /// <summary>
    /// Seeds a key-value setting into the emulator.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">The configuration value.</param>
    /// <param name="label">The label (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task SeedAsync(string key, string value, string label = null, CancellationToken cancellationToken = default)
        => await Client.AddConfigurationSettingAsync(key, value, label, cancellationToken);

    /// <inheritdoc />
    public ValueTask DisposeAsync() => Container.DisposeAsync();
}
