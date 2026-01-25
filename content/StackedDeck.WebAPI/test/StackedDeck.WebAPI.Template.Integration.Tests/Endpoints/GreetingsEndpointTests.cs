using System.Net;
#if (UseFastEndpoints)
using System.Text.Json;
#endif
using System.Threading.Tasks;

using Shouldly;

#if (UseFastEndpoints)
using StackedDeck.WebAPI.Template.API.Endpoints.Greetings;
#endif
using StackedDeck.WebAPI.Template.Integration.Tests.Fixtures;

using static StackedDeck.WebAPI.Template.API.Constants.Api;

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
            .GetAsync($"{Routes.PREFIX}/{Routes.Versioning.V1_SET}/greetings", TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(async void () =>
        {
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
#if (UseFastEndpoints)
            var responseContent = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<GreetingsResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            response.ShouldNotBeNull();
            response.Message.ShouldBe("Buongiorno!");
#else
            (await result.Content.ReadAsStringAsync()).ShouldBe("Buongiorno!");
#endif
        });
    }
}
