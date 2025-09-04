using Nuke.Common;
using Nuke.Common.Tools.GitVersion;

namespace Components;

internal interface IHasGitVersion : INukeBuild
{
    [GitVersion(UpdateBuildNumber = true, NoFetch = true)]
    GitVersion GitVersion => TryGetValue(() => GitVersion);

    string SemanticVersion => GitVersion.SemVer;
}
