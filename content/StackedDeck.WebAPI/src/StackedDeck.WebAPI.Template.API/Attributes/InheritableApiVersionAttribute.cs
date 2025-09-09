using System;

using Asp.Versioning;

namespace StackedDeck.WebAPI.Template.API.Attributes;

/// <summary>
/// Represents the metadata that describes the <see cref="ApiVersion">versions</see> associated with an API. Unlike
/// <see cref="ApiVersionAttribute"/>, this attribute is inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
internal class InheritableApiVersionAttribute : ApiVersionsBaseAttribute, IApiVersionProvider
{
    private ApiVersionProviderOptions options = ApiVersionProviderOptions.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="InheritableApiVersionAttribute"/> class.
    /// </summary>
    /// <param name="version">The API version string.</param>
    public InheritableApiVersionAttribute(double version) : base(version)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InheritableApiVersionAttribute"/> class.
    /// </summary>
    /// <param name="version">A numeric API version.</param>
    /// <param name="status">The status associated with the API version, if any.</param>
    public InheritableApiVersionAttribute(double version, string status)
        : base(new ApiVersion(version, status))
    {
    }

    ApiVersionProviderOptions IApiVersionProvider.Options => options;

    /// <summary>
    /// Gets or sets a value indicating whether the specified set of API versions are deprecated.
    /// </summary>
    /// <value>True if the specified set of API versions are deprecated; otherwise, false.
    /// The default value is <c>false</c>.</value>
    public bool Deprecated
    {
        get => (options & ApiVersionProviderOptions.Deprecated) == ApiVersionProviderOptions.Deprecated;
        set
        {
            if (value)
            {
                options |= ApiVersionProviderOptions.Deprecated;
            }
            else
            {
                options &= ~ApiVersionProviderOptions.Deprecated;
            }
        }
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Deprecated);

    public override bool Equals(object obj) => base.Equals(obj) && obj?.GetHashCode() == GetHashCode();

    protected InheritableApiVersionAttribute(ApiVersion version)
        : base(version)
    {
    }

    protected InheritableApiVersionAttribute(ApiVersion version, params ApiVersion[] otherVersions)
        : base(version, otherVersions)
    {
    }

    protected InheritableApiVersionAttribute(double version, params double[] otherVersions)
        : base(version, otherVersions)
    {
    }

    protected InheritableApiVersionAttribute(string version)
        : base(version)
    {
    }

    protected InheritableApiVersionAttribute(string version, params string[] otherVersions)
        : base(version, otherVersions)
    {
    }

    protected InheritableApiVersionAttribute(IApiVersionParser parser, string version)
        : base(parser, version)
    {
    }

    protected InheritableApiVersionAttribute(IApiVersionParser parser, string version, params string[] otherVersions)
        : base(parser, version, otherVersions)
    {
    }
}
