using Nuke.Common;
using Nuke.Common.Git;

namespace Components;

internal interface IHasGitRepository : INukeBuild
{
    [GitRepository]
    GitRepository GitRepository => TryGetValue(() => GitRepository);
}
