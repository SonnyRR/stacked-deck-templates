using System.IO;

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.ReportGenerator;

using Serilog;

namespace Components;

internal interface ICodeCoverage : IHasCodeCoverageArtifacts
{
    Target GenerateCoverageReport => _ => _
        .Description("Generates human-readable code coverage reports from Cobertura XML files.")
        .TriggeredBy<IDotNet>(t => t.Test)
        .Executes(() =>
        {
            var coberturaCoverageFiles = CoverageDirectory.GlobFiles("**/coverage.cobertura.xml");

            if (coberturaCoverageFiles.Count == 0)
            {
                Log.Warning(
                    "No Cobertura coverage reports were found in {CoverageDirectory}. Skipping report generation.",
                    CoverageDirectory);
            }

            ReportGeneratorTasks.ReportGenerator(s => s
                .SetReports(CoverageDirectory / "**" / "coverage.cobertura.xml")
                .SetTargetDirectory(CoverageReportsDirectory)
                .SetReportTypes(ReportTypes.HtmlInline, ReportTypes.MarkdownSummaryGithub));
        });

    Target PublishCoverageSummary => _ => _
        .OnlyWhenStatic(() => IsServerBuild)
        .TriggeredBy(GenerateCoverageReport)
        .Unlisted()
        .Executes(async () =>
        {
            var summaryFile = CoverageReportsDirectory / "SummaryGithub.md";

            if (!File.Exists(summaryFile))
            {
                Log.Warning(
                    "No code coverage summary found at {SummaryContent}. Skipping summary publication.",
                    summaryFile);
            }

            var summaryContent = await File.ReadAllTextAsync(summaryFile);

#if (UseGitHubActions)
            var stepSummaryPath = EnvironmentInfo.GetVariable<string>("GITHUB_STEP_SUMMARY");

            if (string.IsNullOrWhiteSpace(stepSummaryPath))
            {
                Log.Warning("GITHUB_STEP_SUMMARY environment variable is not set. Skipping summary publication.");
                return;
            }

            var processedSummaryContent = summaryContent.Replace("# Summary", "## 📊 Test Coverage Summary");

            await File.AppendAllTextAsync(stepSummaryPath, processedSummaryContent);
#else
            // TODO: Add your own logic to publish the code coverage summary,
            // according to your provider of choice.
#endif
        });
}
