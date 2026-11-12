using System;
#if (UseOTELCollector)
using System.Diagnostics;
#endif
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace StackedDeck.WebAPI.Template.API.Middlewares;

/// <summary>
/// Custom middleware, that assigns a correlation
/// identifier to every incoming request.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate next;

    /// <summary>
    /// Constructs a new instance of <see cref="CorrelationIdMiddleware" />.
    /// </summary>
    public CorrelationIdMiddleware(RequestDelegate next) => this.next = next;

    /// <summary>
    /// Resolves the correlation identifier for the incoming request and
    /// appends it to the HTTP context's request &amp; response objects.
    /// <para>
    /// A correlation identifier supplied by the consumer takes precedence over a
    /// newly generated one. The resolved identifier is also attached to the
    /// incoming request's trace, so that requests can be correlated within the
    /// tracing backend.
    /// </para>
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var correlationId = ResolveCorrelationId(context.Request.Headers);

        context.Request.Headers.TryAdd(Constants.Headers.CORRELATION_ID, correlationId);

#if (UseOTELCollector)
        Activity.Current?.SetTag(Constants.Tracing.CORRELATION_ID, correlationId);

#endif
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(Constants.Headers.CORRELATION_ID, correlationId);

            return Task.CompletedTask;
        });

        await next(context);
    }

    /// <summary>
    /// Resolves the correlation identifier for a request. A consumer-supplied
    /// correlation identifier takes precedence over a newly generated one.
    /// </summary>
    /// <param name="headers">The request headers.</param>
    /// <returns>The correlation identifier for the request.</returns>
    private static string ResolveCorrelationId(IHeaderDictionary headers) =>
        headers.TryGetValue(Constants.Headers.CORRELATION_ID, out var incoming) &&
        !StringValues.IsNullOrEmpty(incoming)
            ? incoming.ToString()
            : Guid.NewGuid().ToString();
}
