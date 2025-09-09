using System;

using Azure.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

using StackedDeck.WebAPI.Template.API.Configuration;
using StackedDeck.WebAPI.Template.Common.Extensions;

namespace StackedDeck.WebAPI.Template.API.Extensions;

/// <summary>
/// Provides extension methods to integrate Azure App Configuration with an IHostBuilder.
/// </summary>
public static class AzureAppConfigurationExtensions
{
    /// <summary>
    /// Adds Azure App Configuration settings to the provided <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The configuration builder to which Azure App Configuration should be added.</param>
    /// <returns>
    /// The updated <see cref="IConfigurationBuilder"/> instance with Azure App Configuration integrated.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="builder"/> is null.</exception>
    /// <exception cref="UriFormatException">Thrown when the App Configuration endpoint is not a valid URI.</exception>
    public static IHostBuilder AddAzureAppConfiguration(this IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureAppConfiguration((context, config) =>
        {
            if (context.HostingEnvironment.IsLocal() || context.HostingEnvironment.IsE2E())
            {
                // If the environment is 'Local', we do not want to use Azure App Configuration.
                // We're going to rely on the appsettings.Local.json document instead.
                return;
            }

            // Usually we would want to retrieve options from the DI container, via the IOptions pattern.
            // However, in this case we need to read the configuration before the DI container is built, therefore
            // we cannot rely on performing the validations, that are usually configured during the option registration.
            // If the 'AppConfigEndpoint' doesn't have a value, the application will fail to start either way.
            // In order to avoid duplicating the service provider build, we will use the configuration directly
            // and let the bootstrap process fail if the configuration is not valid.
            var configurationRoot = config.Build();

            var managedIdentityOptions = configurationRoot
                .GetSection(ManagedIdentityOptions.CFG_SECTION_NAME)
                .Get<ManagedIdentityOptions>();

            if (string.IsNullOrWhiteSpace(managedIdentityOptions?.AppConfigEndpoint))
            {
                throw new InvalidOperationException("Azure App Configuration endpoint is not configured. " +
                    "Please set the 'ManagedIdentity:AppConfigEndpoint' value in your configuration.");
            }

            var apiOptions = configurationRoot
                .GetSection(ApiOptions.CFG_SECTION_NAME)
                .Get<ApiOptions>();

            if (string.IsNullOrWhiteSpace(apiOptions?.Identifier))
            {
                throw new InvalidOperationException("API identifier value is not configured. " +
                    "Please set the 'API:Identifier' value in your configuration.");
            }

            config.AddAzureAppConfiguration(options =>
            {
                var credential = new DefaultAzureCredential();

                options.Connect(new Uri(managedIdentityOptions?.AppConfigEndpoint), credential)
                    .Select(KeyFilter.Any, LabelFilter.Null)
                    .Select(KeyFilter.Any, $"{apiOptions.Identifier}-{context.HostingEnvironment.EnvironmentName}")
                    .ConfigureKeyVault(kvo => kvo.SetCredential(credential));
            });
        });

        return builder;
    }
}
