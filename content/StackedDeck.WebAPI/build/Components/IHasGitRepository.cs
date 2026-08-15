using Fallout.Common;
using Fallout.Common.Git;

namespace Components;

internal interface IHasGitRepository : IFalloutBuild
{
    [GitRepository]
    GitRepository GitRepository => TryGetValue(() => GitRepository);
}
