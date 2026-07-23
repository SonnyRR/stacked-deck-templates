using Fallout.Common;
using Fallout.Common.ProjectModel;

namespace Components;

internal interface IHasSolution : IFalloutBuild
{
    [Solution]
    Solution Solution => TryGetValue(() => Solution);
}
