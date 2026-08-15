using Components;

using Fallout.Common;

namespace StackedDeck.WebAPI.Template.Build;

/// <summary>
/// The main FALLOUT build system entry point.
/// </summary>
public class Build :
    FalloutBuild,
    IDotNet,
    ICodeCoverage,
    IDocker,
    IVersionArtifacts
{
    /// <summary>
    /// The default FALLOUT build target.
    /// </summary>
    /// <returns>The exit code of the build process.</returns>
    public static int Main() => Execute<Build>(x => ((IDotNet)x).Build);
}
