using Nuke.Common;
using Nuke.Common.ProjectModel;

namespace Components;

internal interface IHasSolution : INukeBuild
{
    [Solution]
    Solution Solution => TryGetValue(() => Solution);
}
