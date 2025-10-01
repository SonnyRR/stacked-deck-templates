using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StackedDeck.WebAPI.Template.API.Extensions;

/// <summary>
/// Contains utility methods that extend <see cref="ProblemDetails"/> with custom metadata.
/// </summary>
public static class ProblemDetailsExtensions
{
    /// <summary>
    /// Extends the <see cref="ProblemDetails"/> instance with HTTP metadata.
    /// </summary>
    /// <param name="problemDetails">The problem details instance.</param>
    /// <param name="httpContext">
    /// The HTTP context, that contains the additional metadata.
    /// </param>
    /// <returns>
    /// An extended instance of <see cref="ProblemDetails"/> with HTTP metadata.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    public static ProblemDetails WithHttpContextMetadata(this ProblemDetails problemDetails, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);
        ArgumentNullException.ThrowIfNull(httpContext);

        if (httpContext.Request.Headers.TryGetValue(Constants.Headers.CORRELATION_ID, out var correlationId))
        {
            problemDetails.Extensions.TryAdd(nameof(correlationId), correlationId.ToString());
        }

        problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";

        return problemDetails;
    }

    /// <summary>
    /// Assigns a HTTP status code to a <see cref="ProblemDetails"/> instance.
    /// </summary>
    /// <param name="problemDetails">The problem details instance.</param>
    /// <param name="httpStatusCode">The HTTP status code to assign.</param>
    /// <returns>
    /// An instance of <see cref="ProblemDetails"/> with an assigned HTTP status code.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    public static ProblemDetails WithHttpStatusCode(this ProblemDetails problemDetails, int httpStatusCode)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);

        problemDetails.Status = httpStatusCode;

        return problemDetails;
    }

    /// <summary>
    /// Assigns a title to a <see cref="ProblemDetails"/> instance.
    /// </summary>
    /// <param name="problemDetails">The problem details instance.</param>
    /// <param name="title">The title to assign.</param>
    /// <returns>
    /// An instance of <see cref="ProblemDetails"/> with an assigned title.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    public static ProblemDetails WithTitle(this ProblemDetails problemDetails, string title)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        problemDetails.Title = title;

        return problemDetails;
    }
}
