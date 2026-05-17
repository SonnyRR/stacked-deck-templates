using StackedDeck.WebAPI.Template.Integration.Tests.Fixtures;

#if (UseAzureCloudProvider)
[assembly: AssemblyFixture(typeof(AzureAppConfigurationFixture))]
#endif
[assembly: AssemblyFixture(typeof(ApiFixture))]
