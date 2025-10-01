using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using StackedDeck.WebAPI.Template.API.Extensions;

namespace StackedDeck.WebAPI.Template.API.Handlers;

/// <summary>
/// Global exception handler, that will log the unhandled exception
/// and return a HTTP 500 response via <see cref="ProblemDetails"/>.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    /// <summary>
    /// Creates a new instance of <see cref="GlobalExceptionHandler"/>.
    /// </summary>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => this.logger = logger;

    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        logger.LogError(exception, "Exception thrown: {Message}", exception.Message);

        var problemDetails = new ProblemDetails()
            .WithTitle("Server error")
            .WithHttpStatusCode(StatusCodes.Status500InternalServerError)
            .WithHttpContextMetadata(httpContext);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
