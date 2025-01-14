using System.Net;
using System.Threading.Tasks;

using Shouldly;

using StackedDeck.WebAPI.Template.API.Controllers;
using StackedDeck.WebAPI.Template.Integration.Tests.Fixtures;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Controllers;

/// <summary>
/// Contains integration tests for the <see cref="GreetingsController"/> class.
/// </summary>
public class GreetingsControllerTests
{
    private readonly ApiFixture apiFixture;

    /// <summary>
    /// Creates a new instance of <see cref="GreetingsControllerTests"/>.
    /// </summary>
    public GreetingsControllerTests(ApiFixture apiFixture)
    {
        this.apiFixture = apiFixture;
    }

    /// <summary>
    /// Tests whether the <see cref="GreetingsController.Greet"/> method
    /// returns an OK response with the expected greeting content.
    /// </summary>
    [Fact]
    public async Task GetGreet_ReturnsOK_WithAnItalianGreeting()
    {
        // Act
        var result = await apiFixture.Client.GetAsync("/sd-api-route-prefix/v1/greetings", TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(async void () =>
        {
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await result.Content.ReadAsStringAsync()).ShouldBe("Buongiorno!");
        });
    }
}
