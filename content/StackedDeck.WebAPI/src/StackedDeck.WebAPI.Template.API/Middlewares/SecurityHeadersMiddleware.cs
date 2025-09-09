using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace StackedDeck.WebAPI.Template.API.Middlewares;

/// <summary>
/// Assigns common security headers to each response.
/// </summary>
/// <remarks>
/// Check https://cheatsheetseries.owasp.org/cheatsheets/REST_Security_Cheat_Sheet.html#security-headers
/// for reference &amp; in-depth explanation.
/// </remarks>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate next;

    /// <summary>
    /// Creates a new instance of the <see cref="SecurityHeadersMiddleware"/>.
    /// </summary>
    /// <param name="next">The following request delegate in the middleware pipeline.</param>
    public SecurityHeadersMiddleware(RequestDelegate next) => this.next = next;

    /// <summary>
    /// Appends the recommended security headers from the OWASP REST Security cheat sheet.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(HeaderNames.ContentSecurityPolicy, "frame-ancestors 'none'");
            context.Response.Headers.Append(HeaderNames.XFrameOptions, "DENY");
            context.Response.Headers.Append(HeaderNames.XContentTypeOptions, "nosniff");
            context.Response.Headers.Append(HeaderNames.StrictTransportSecurity, "max-age=31536000; includeSubDomains");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer, origin-when-cross-origin");

            return Task.CompletedTask;
        });

        await next(context);
    }
}
