using System;
using System.Net.Http;

using StackedDeck.WebAPI.Template.API;
using StackedDeck.WebAPI.Template.Integration.Tests.Factories;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Fixtures;

/// <summary>
/// Fixture for communicating with APIs through REST HTTP reqeusts.
/// </summary>
/// <remarks>
/// Should be registered as a global fixture in order to re-use resources
/// such as the 'HttpClient' which is a costly resource to re-create constantly
/// and can lead to socket exhaustion. It will also set up the web host once
/// until all tests are executed in a given run.
/// </remarks>
public class ApiFixture : IDisposable
{
    /// <summary>
    /// The API factory used to construct the web host for the SUT.
    /// </summary>
    public ApiFactory<Program> ApiFactory { get; }

    /// <summary>
    /// The HTTP client used to interact with the SUT.
    /// </summary>
    public HttpClient Client { get; }

    /// <summary>
    /// Creates a new instance of <see cref="ApiFixture"/>
    /// </summary>
    public ApiFixture()
    {
        ApiFactory = new();
        Client = ApiFactory.CreateClient();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// A disposal method, which adheres to RSPEC-3881
    /// </summary>
    /// <param name="disposing">
    /// A flag for more granular control over how the resources should be cleaned-up.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        ApiFactory?.Dispose();
        Client?.Dispose();
    }
}
