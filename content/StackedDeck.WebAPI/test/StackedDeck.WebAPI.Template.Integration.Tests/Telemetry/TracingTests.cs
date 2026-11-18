using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Shouldly;

using StackedDeck.WebAPI.Template.Integration.Tests.Fixtures;
using StackedDeck.WebAPI.Template.Integration.Tests.Utils;

using static StackedDeck.WebAPI.Template.API.Constants;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Telemetry;

/// <summary>
/// Integration tests for the tracing pipeline, asserted on the spans it hands to the in-memory exporter.
/// </summary>
/// <remarks>
/// Every request carries a sampled W3C <c>traceparent</c> with a fresh trace id, so a test only ever looks at the
/// spans of its own requests, whatever else the host traces in the meantime.
/// </remarks>
public class TracingTests
{
    private const string HEALTH_ROUTE = Api.Routes.PREFIX + "/health";
    private const string CORRELATION_ID_HEADER = Headers.CORRELATION_ID;
    private const string CORRELATION_ID_ATTRIBUTE = Tracing.CORRELATION_ID;

    private static readonly TimeSpan PROPAGATION_TIMEOUT = TimeSpan.FromSeconds(10);

    /// <summary>
    /// The API fixture used to communicate with the SUT.
    /// </summary>
    private readonly ApiFixture apiFixture;

    /// <summary>
    /// Creates a new instance of <see cref="TracingTests"/>.
    /// </summary>
    /// <param name="apiFixture">The API fixture.</param>
    public TracingTests(ApiFixture apiFixture)
        => this.apiFixture = apiFixture;

    /// <summary>
    /// Verifies that a request continues the caller's trace with a server span.
    /// </summary>
    [Fact(DisplayName = "A request exports a server span in the caller's trace")]
    public async Task Request_SampledTraceParent_ExportsServerSpan()
    {
        // Arrange
        var traceId = ActivityTraceId.CreateRandom();

        // Act
        using var response = await SendWithTraceParentAsync(HEALTH_ROUTE, traceId);
        var serverSpan = await WaitForServerSpanAsync(traceId);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        serverSpan.GetTagItem("url.path").ShouldBe(HEALTH_ROUTE);
    }

    /// <summary>
    /// Verifies that the correlation identifier supplied by the consumer is attached to the server span and echoed
    /// back exactly once.
    /// </summary>
    [Fact(DisplayName = "A request's correlation identifier is attached to its server span")]
    public async Task Request_WithCorrelationId_TagsServerSpan()
    {
        // Arrange
        var traceId = ActivityTraceId.CreateRandom();
        var correlationId = Guid.NewGuid().ToString();

        // Act
        using var response = await SendWithTraceParentAsync(HEALTH_ROUTE, traceId, correlationId);
        var serverSpan = await WaitForServerSpanAsync(traceId);

        // Assert
        serverSpan.ShouldSatisfyAllConditions(
            () => serverSpan.GetTagItem(CORRELATION_ID_ATTRIBUTE).ShouldBe(correlationId),
            () => response.Headers.GetValues(CORRELATION_ID_HEADER).ShouldBe([correlationId]));
    }

    /// <summary>
    /// Verifies that a request without a correlation identifier gets a generated one, which is attached to the server
    /// span and returned to the consumer.
    /// </summary>
    [Fact(DisplayName = "A request without a correlation identifier gets a generated one on its server span")]
    public async Task Request_WithoutCorrelationId_TagsServerSpanWithGeneratedId()
    {
        // Arrange
        var traceId = ActivityTraceId.CreateRandom();

        // Act
        using var response = await SendWithTraceParentAsync(HEALTH_ROUTE, traceId);
        var serverSpan = await WaitForServerSpanAsync(traceId);

        // Assert
        var correlationId = response.Headers.GetValues(CORRELATION_ID_HEADER).ShouldHaveSingleItem();

        serverSpan.ShouldSatisfyAllConditions(
            () => Guid.TryParse(correlationId, out _).ShouldBeTrue(),
            () => serverSpan.GetTagItem(CORRELATION_ID_ATTRIBUTE).ShouldBe(correlationId));
    }

    /// <summary>
    /// Sends a GET request, which continues the given trace via a sampled W3C <c>traceparent</c>, optionally
    /// carrying a correlation identifier.
    /// </summary>
    /// <param name="requestUri">The URI to request.</param>
    /// <param name="traceId">The trace id the request should continue.</param>
    /// <param name="correlationId">The optional correlation identifier to carry.</param>
    /// <returns>The response of the SUT.</returns>
    private async Task<HttpResponseMessage> SendWithTraceParentAsync(
        string requestUri,
        ActivityTraceId traceId,
        string correlationId = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.TryAddWithoutValidation("traceparent", $"00-{traceId}-{ActivitySpanId.CreateRandom()}-01");

        if (correlationId is not null)
        {
            request.Headers.Add(CORRELATION_ID_HEADER, correlationId);
        }

        return await apiFixture.Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    /// <summary>
    /// Polls the exported spans until the server span of the given trace shows up.
    /// </summary>
    /// <param name="traceId">The trace id to look the server span up by.</param>
    /// <returns>The server span of the given trace.</returns>
    private async Task<Activity> WaitForServerSpanAsync(ActivityTraceId traceId)
    {
        Activity serverSpan = null;

        await AsyncPolling.WaitForAsync(
            () =>
            {
                serverSpan = ReadSpans(traceId).FirstOrDefault(span => span.Kind is ActivityKind.Server);
                return Task.FromResult(serverSpan is not null);
            },
            PROPAGATION_TIMEOUT);

        return serverSpan;
    }

    /// <summary>
    /// Reads all spans the SUT exported for the given trace, as a point-in-time snapshot.
    /// </summary>
    /// <param name="traceId">The trace id to read the spans by.</param>
    /// <returns>The spans of the given trace, in the order they were exported.</returns>
    private List<Activity> ReadSpans(ActivityTraceId traceId)
        =>
        [
            .. apiFixture.ApiFactory.ExportedActivities
                .Snapshot()
                .Where(span => span.TraceId == traceId)
        ];
}
