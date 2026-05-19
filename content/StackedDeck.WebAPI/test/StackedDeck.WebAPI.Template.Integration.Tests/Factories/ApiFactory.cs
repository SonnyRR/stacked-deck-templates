using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using StackedDeck.WebAPI.Template.Common;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Factories;

/// <summary>
/// A generic API host factory, utilizing the E2E environment.
/// </summary>
public class ApiFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null, EnvironmentVariableTarget.Process);

        builder.UseEnvironment(Environments.E2E);
        base.ConfigureWebHost(builder);
    }
}
