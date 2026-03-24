using Nuke.Common;
using Nuke.Common.IO;

namespace Components;

internal interface IHasCodeCoverageArtifacts : INukeBuild
{
    const string COVERAGE_DIR_NAME = "coverage";

    AbsolutePath CoverageDirectory => RootDirectory / COVERAGE_DIR_NAME;

    AbsolutePath CoverageReportsDirectory => CoverageDirectory / "reports";
}
