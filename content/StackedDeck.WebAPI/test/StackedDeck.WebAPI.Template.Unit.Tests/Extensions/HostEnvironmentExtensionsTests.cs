using System.Collections;

using Microsoft.Extensions.Hosting;

using Moq;

using Shouldly;

using StackedDeck.WebAPI.Template.Common.Extensions;

using Environments = StackedDeck.WebAPI.Template.Common.Environments;
namespace StackedDeck.WebAPI.Template.Unit.Tests.Extensions;

/// <summary>
/// Unit tests for the <see cref="HostEnvironmentExtensions"/> class.
/// </summary>
public class HostEnvironmentExtensionsTests
{
    /// <summary>
    /// The mocked <see cref="IHostEnvironment"/> instance used for testing.
    /// </summary>
    private readonly Mock<IHostEnvironment> mockedEnvironment;

    /// <summary>
    /// Creates a new instance of the <see cref="HostEnvironmentExtensionsTests"/> class with
    /// a mocked <see cref="IHostEnvironment"/>.
    /// </summary>
    public HostEnvironmentExtensionsTests() => mockedEnvironment = new Mock<IHostEnvironment>();

    /// <summary>
    /// Ensure that the <see cref="HostEnvironmentExtensions.IsE2E(IHostEnvironment)"/>
    /// extension method correctly identifies the E2E environment.
    /// </summary>
    /// <param name="environment">The environment identifier.</param>
    /// <param name="isExpectedEnvironment">Boolean flag, indicating if the expected environment is correct.</param>
    [Theory]
    [ClassData(typeof(E2EEnvironmentTestData))]
    public void IsE2E_WhenEnvironmentIsE2E_ReturnsCorrectFlag(string environment, bool isExpectedEnvironment)
    {
        // Arrange
        mockedEnvironment.Setup(e => e.EnvironmentName).Returns(environment);
        // Act
        var result = mockedEnvironment.Object.IsE2E();

        // Assert
        result.ShouldBe(isExpectedEnvironment);
    }

    /// <summary>
    /// Ensure that the <see cref="HostEnvironmentExtensions.IsLocal(IHostEnvironment)"/>
    /// extension method correctly identifies the Local environment.
    /// </summary>
    /// <param name="environment">The environment identifier.</param>
    /// <param name="isExpectedEnvironment">Boolean flag, indicating if the expected environment is correct.</param>
    [Theory]
    [ClassData(typeof(LocalEnvironmentTestData))]
    public void IsLocal_WhenEnvironmentIsLocal_ReturnsCorrectFlag(string environment, bool isExpectedEnvironment)
    {
        // Arrange
        mockedEnvironment.Setup(e => e.EnvironmentName).Returns(environment);

        // Act
        var result = mockedEnvironment.Object.IsLocal();

        // Assert
        result.ShouldBe(isExpectedEnvironment);
    }
}

internal class E2EEnvironmentTestData : IEnumerable<TheoryDataRow<string, bool>>
{
    public IEnumerator<TheoryDataRow<string, bool>> GetEnumerator()
    {
        yield return new(Environments.E2E, true);
        yield return new(Environments.Development, false);
        yield return new(Environments.Production, false);
        yield return new(Environments.Local, false);
        yield return new(Environments.Staging, false);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal class LocalEnvironmentTestData : IEnumerable<TheoryDataRow<string, bool>>
{
    public IEnumerator<TheoryDataRow<string, bool>> GetEnumerator()
    {
        yield return new(Environments.E2E, false);
        yield return new(Environments.Development, false);
        yield return new(Environments.Production, false);
        yield return new(Environments.Local, true);
        yield return new(Environments.Staging, false);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
