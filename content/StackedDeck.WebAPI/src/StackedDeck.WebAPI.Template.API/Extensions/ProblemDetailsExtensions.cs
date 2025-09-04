using System;
using System.Collections.Generic;
using System.Net;

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
        problemDetails.WithType();

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

    /// <summary>
    /// Assigns a HTTP response type URI from RFC9110.
    /// </summary>
    /// <remarks>
    /// The type URI is assigned based on the <see cref="ProblemDetails.Status"/>
    /// value. If the value is missing it will default to 'about:blank'.
    /// </remarks>
    /// <param name="problemDetails">The problem details instance.</param>
    /// <returns>
    /// An instance of <see cref="ProblemDetails"/> with an assigned type.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    private static ProblemDetails WithType(this ProblemDetails problemDetails)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);

        // Extend with additional cases if you are going to make extensive use of other
        // HTTP 4xx or HTTP 5xx status codes.
        problemDetails.Type = (HttpStatusCode)problemDetails.Status.GetValueOrDefault() switch
        {
            HttpStatusCode.InternalServerError => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
            HttpStatusCode.Conflict => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10",
            HttpStatusCode.NotFound => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
            HttpStatusCode.Forbidden => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4",
            HttpStatusCode.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2",
            HttpStatusCode.BadRequest => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1",
            _ => "about:blank"
        };

        return problemDetails;
    }
}
