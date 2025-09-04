using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace StackedDeck.WebAPI.Template.API.Middlewares;

/// <summary>
/// Custom middleware, that attaches a unique correlation
/// identifier to the incoming requests.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate next;

    /// <summary>
    /// Constructs a new instance of <see cref="CorrelationIdMiddleware" />.
    /// </summary>
    public CorrelationIdMiddleware(RequestDelegate next) => this.next = next;

    /// <summary>
    /// Appends a custom correlation identifier header to the
    /// HTTP context's request object.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();

        context.Request.Headers.TryAdd(Constants.Headers.CORRELATION_ID, correlationId);
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(Constants.Headers.CORRELATION_ID, correlationId);

            return Task.CompletedTask;
        });

        await next(context);
    }
}
