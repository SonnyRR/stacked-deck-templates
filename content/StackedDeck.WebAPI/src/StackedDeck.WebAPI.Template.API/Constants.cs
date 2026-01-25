namespace StackedDeck.WebAPI.Template.API;

/// <summary>
/// Utility class, that holds API level constants.
/// </summary>
public static class Constants
{

    /// <summary>
    /// Constants, related to HTTP headers.
    /// </summary>
    public static class Headers
    {
        /// <summary>
        /// The correlation identifier header.
        /// </summary>
        public const string CORRELATION_ID = "x-correlation-id";
    }

    /// <summary>
    /// Constants, related to the API.
    /// </summary>
    public static class Api
    {
        /// <summary>
        /// Constants, related to the API routes.
        /// </summary>
        public static class Routes
        {
            /// <summary>
            /// The global route prefix for this API.
            /// </summary>
            public const string PREFIX = "/sd-api-route-prefix";

            /// <summary>
            /// Constants, related to the API versions.
            /// </summary>
            public static class Versioning
            {
                /// <summary>
                /// The default API version set.
                /// </summary>
                public const string V1_SET = "v1";
            }
        }
    }
}
