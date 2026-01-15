using System.Net;
using System.Threading.Tasks;

using Shouldly;

using StackedDeck.WebAPI.Template.Integration.Tests.Fixtures;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Endpoints;

/// <summary>
/// Contains integration tests for the Greetings minimal API endpoint.
/// </summary>
public class GreetingsEndpointTests
{
    private readonly ApiFixture apiFixture;

    /// <summary>
    /// Creates a new instance of <see cref="GreetingsEndpointTests"/>.
    /// </summary>
    public GreetingsEndpointTests(ApiFixture apiFixture)
        => this.apiFixture = apiFixture;

    /// <summary>
    /// Tests whether the greetings minimal API endpoint
    /// returns an OK response with the expected greeting content.
    /// </summary>
    [Fact(DisplayName = "Greetings endpoint returns OK with Italian greeting")]
    public async Task GetGreetings_ReturnsOK_WithAnItalianGreeting()
    {
        // Act
        var result = await apiFixture
            .Client
            .GetAsync("/sd-api-route-prefix/v1/greetings", TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(async void () =>
        {
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await result.Content.ReadAsStringAsync()).ShouldBe("Buongiorno!");
        });
    }
}
