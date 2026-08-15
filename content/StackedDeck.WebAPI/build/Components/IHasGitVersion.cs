using Fallout.Common;
using Fallout.Common.Tools.GitVersion;

namespace Components;

internal interface IHasGitVersion : IFalloutBuild
{
    [GitVersion(UpdateBuildNumber = true, NoFetch = true)]
    GitVersion GitVersion => TryGetValue(() => GitVersion);

    string SemanticVersion => GitVersion.SemVer;

    bool IsPullRequest => SemanticVersion.Contains("PullRequest");
}
