using Components;

using Nuke.Common;

namespace StackedDeck.WebAPI.Template.Build;

/// <summary>
/// The main NUKE build system entry point.
/// </summary>
public class Build :
    NukeBuild,
    IDotNet,
    IDocker,
    IVersionArtifacts
{
    /// <summary>
    /// The default NUKE build target.
    /// </summary>
    /// <returns>The exit code of the build process.</returns>
    public static int Main() => Execute<Build>(x => ((IDotNet)x).Build);
}
