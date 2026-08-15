using Fallout.Common;
using Fallout.Solutions;

namespace Components;

internal interface IHasSolution : IFalloutBuild
{
    [Solution]
    Solution Solution => TryGetValue(() => Solution);
}
