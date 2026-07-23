using Fallout.Common;
using Fallout.Common.IO;

namespace Components;

internal interface IHasCodeCoverageArtifacts : IFalloutBuild
{
    const string COVERAGE_DIR_NAME = "coverage";

    AbsolutePath CoverageDirectory => RootDirectory / COVERAGE_DIR_NAME;

    AbsolutePath CoverageReportsDirectory => CoverageDirectory / "reports";
}
