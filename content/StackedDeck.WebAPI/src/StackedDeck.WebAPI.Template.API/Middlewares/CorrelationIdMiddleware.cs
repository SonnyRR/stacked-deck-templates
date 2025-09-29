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
        context.Request.Headers.TryAdd(Constants.Headers.CORRELATION_ID, Guid.NewGuid().ToString());

        await next(context);
    }
}
