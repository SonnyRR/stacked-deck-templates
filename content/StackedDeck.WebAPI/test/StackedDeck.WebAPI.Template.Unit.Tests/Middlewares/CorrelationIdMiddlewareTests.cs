using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using Moq;

using Shouldly;

using StackedDeck.WebAPI.Template.API;
using StackedDeck.WebAPI.Template.API.Middlewares;

namespace StackedDeck.WebAPI.Template.Unit.Tests.Middlewares;

/// <summary>
/// Unit tests for the <see cref="CorrelationIdMiddleware" /> class.
/// </summary>
public class CorrelationIdMiddlewareTests
{
    /// <summary>
    /// The mocked <see cref="HttpContext" /> instance used for testing.
    /// </summary>
    private readonly Mock<HttpContext> mockedContext;

    /// <summary>
    /// The mocked <see cref="HttpRequest" /> instance, exposed by the mocked context.
    /// </summary>
    private readonly Mock<HttpRequest> mockedRequest;

    /// <summary>
    /// The mocked <see cref="HttpResponse" /> instance, exposed by the mocked context.
    /// </summary>
    private readonly Mock<HttpResponse> mockedResponse;

    /// <summary>
    /// The request headers collection, backing the mocked request.
    /// </summary>
    private readonly HeaderDictionary requestHeaders;

    /// <summary>
    /// The response headers collection, backing the mocked response.
    /// </summary>
    private readonly HeaderDictionary responseHeaders;

    /// <summary>
    /// The captured response starting callback, registered by the middleware.
    /// </summary>
    private Func<Task> responseStartingCallback;

    /// <summary>
    /// Creates a new instance of the <see cref="CorrelationIdMiddlewareTests" /> class
    /// with a mocked <see cref="HttpContext" />.
    /// </summary>
    public CorrelationIdMiddlewareTests()
    {
        requestHeaders = [];
        responseHeaders = [];

        mockedRequest = new Mock<HttpRequest>();
        mockedRequest.SetupGet(r => r.Headers).Returns(requestHeaders);

        mockedResponse = new Mock<HttpResponse>();
        mockedResponse.SetupGet(r => r.Headers).Returns(responseHeaders);
        mockedResponse
            .Setup(r => r.OnStarting(It.IsAny<Func<Task>>()))
                        .Callback<Func<Task>>(callback => responseStartingCallback = callback);

        mockedContext = new Mock<HttpContext>();
        mockedContext.SetupGet(c => c.Request).Returns(mockedRequest.Object);
        mockedContext.SetupGet(c => c.Response).Returns(mockedResponse.Object);
    }

    /// <summary>
    /// Ensure that the middleware throws an <see cref="ArgumentNullException" />,
    /// when the HTTP context is <see langword="null" />.
    /// </summary>
    [Fact(DisplayName = "Middleware throws when the HTTP context is null")]
    public async Task InvokeAsync_NullHttpContext_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new CorrelationIdMiddleware(Next);

        // Act
        var act = async () => await sut.InvokeAsync(null);

        // Assert
        await Should.ThrowAsync<ArgumentNullException>(act);
    }

    /// <summary>
    /// Ensure that the middleware generates a new correlation identifier and
    /// assigns it to the request headers, when the incoming request does not
    /// carry one.
    /// </summary>
    [Fact(DisplayName = "Middleware generates a new correlation identifier, when the request does not carry one")]
    public async Task InvokeAsync_NoIncomingCorrelationId_SetsGeneratedIdOnRequestHeaders()
    {
        // Arrange
        var sut = new CorrelationIdMiddleware(Next);

        // Act
        await sut.InvokeAsync(mockedContext.Object);

        // Assert
        var resolvedId = requestHeaders[Constants.Headers.CORRELATION_ID];
        requestHeaders.Keys.Count(key => key == Constants.Headers.CORRELATION_ID).ShouldBe(1);
        Guid.TryParse(resolvedId.ToString(), out _).ShouldBeTrue();
    }

    /// <summary>
    /// Ensure that the middleware preserves the consumer-supplied correlation
    /// identifier, when the incoming request already carries one.
    /// </summary>
    [Fact(DisplayName = "Middleware preserves the consumer-supplied correlation identifier")]
    public async Task InvokeAsync_IncomingCorrelationId_PreservesIncomingIdOnRequestHeaders()
    {
        // Arrange
        const string INCOMING_ID = "consumer-supplied-id";
        requestHeaders.Append(Constants.Headers.CORRELATION_ID, INCOMING_ID);

        var sut = new CorrelationIdMiddleware(Next);

        // Act
        await sut.InvokeAsync(mockedContext.Object);

        // Assert
        requestHeaders[Constants.Headers.CORRELATION_ID].ToString().ShouldBe(INCOMING_ID);
    }

    /// <summary>
    /// Ensure that the middleware normalizes duplicated incoming correlation
    /// identifiers to a single request header value.
    /// </summary>
    [Fact(DisplayName = "Middleware normalizes duplicated incoming correlation identifiers to a single value")]
    public async Task InvokeAsync_MultipleIncomingCorrelationIds_HoldsResolvedIdExactlyOnce()
    {
        // Arrange
        requestHeaders.Append(Constants.Headers.CORRELATION_ID, "first-id");
        requestHeaders.Append(Constants.Headers.CORRELATION_ID, "second-id");

        var sut = new CorrelationIdMiddleware(Next);

        // Act
        await sut.InvokeAsync(mockedContext.Object);

        // Assert
        var resolvedId = requestHeaders[Constants.Headers.CORRELATION_ID];
        requestHeaders.Keys.Count(key => key == Constants.Headers.CORRELATION_ID).ShouldBe(1);
        resolvedId.Count.ShouldBe(1);
        resolvedId.ToString().ShouldBe("first-id,second-id");
    }

    /// <summary>
    /// Ensure that the middleware registers a response starting callback, which
    /// appends the resolved correlation identifier to the response headers.
    /// </summary>
    [Fact(DisplayName = "Middleware appends the resolved correlation identifier to the response headers")]
    public async Task InvokeAsync_ResponseStarting_AppendsResolvedIdToResponseHeaders()
    {
        // Arrange
        const string INCOMING_ID = "consumer-supplied-id";
        requestHeaders.Append(Constants.Headers.CORRELATION_ID, INCOMING_ID);

        var sut = new CorrelationIdMiddleware(Next);

        // Act
        await sut.InvokeAsync(mockedContext.Object);
        await responseStartingCallback.Invoke();

        // Assert
        responseHeaders[Constants.Headers.CORRELATION_ID].ToString().ShouldBe(INCOMING_ID);
    }

    /// <summary>
    /// Ensure that the middleware invokes the next middleware in the pipeline.
    /// </summary>
    [Fact(DisplayName = "Middleware invokes the next middleware in the pipeline")]
    public async Task InvokeAsync_InvokesNextMiddleware()
    {
        // Arrange
        var nextInvoked = false;
        RequestDelegate next = _ =>
        {
            nextInvoked = true;
            return Task.CompletedTask;
        };

        var sut = new CorrelationIdMiddleware(next);

        // Act
        await sut.InvokeAsync(mockedContext.Object);

        // Assert
        nextInvoked.ShouldBeTrue();
    }

    /// <summary>
    /// Represents the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A completed <see cref="Task" />.</returns>
    private static Task Next(HttpContext context) => Task.CompletedTask;
}
