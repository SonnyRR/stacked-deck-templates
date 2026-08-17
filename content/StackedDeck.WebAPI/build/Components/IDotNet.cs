using System.IO;
using System.Linq;

using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Utilities;

using Serilog;

namespace Components;

internal interface IDotNet : IHasProjects, IHasConfiguration, IHasGitVersion, IHasCodeCoverageArtifacts
{
    const string DATA_COLLECTOR = "XPlat Code Coverage";

    AbsolutePath PublicationDirectory => WebApiProject.Directory / "publish";

    Target Clean => _ => _
        .Description("Cleans-up the compilation & publication artifacts.")
        .Executes(() =>
        {
            var artifactDirectoriesToDelete = RootDirectory
                .GlobDirectories($"**/{{obj,bin,{Path.GetFileName(PublicationDirectory)},{Path.GetFileName(CoverageDirectory)}}}")
                .Where(ap => ap.Parent != BuildProjectDirectory)
                .ToArray();

            if (artifactDirectoriesToDelete.Length == 0)
            {
                Log.Information("✨ Everything is sparkling clean! No artifacts found to delete.");
            }
            else
            {
                foreach (var artifactDir in artifactDirectoriesToDelete)
                {
                    Log.Information("🧹 Deleting directory: {ArtifactDirectory}", artifactDir);
                    artifactDir.DeleteDirectory();
                }
            }
        });

    Target Build => _ => _
        .Description("Builds the assemblies in the solution.")
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Information("🗂️ Solution File: {Solution}", Solution.Path);
            Log.Information("⚙️ Configuration: {Configuration}", Configuration);
            Log.Information("🎯 Assembly Version: {AssemblySemVer}", GitVersion.AssemblySemVer);
            Log.Information("🎯 File Version: {AssemblySemFileVer}", GitVersion.AssemblySemFileVer);
            Log.Information("🎯 Informational Version: {InformationalVersion}", GitVersion.InformationalVersion);
            Log.Information("🎯 Semantic Version: {SemVer}", SemanticVersion);

            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetVersion(SemanticVersion)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .Description("Publishes the API artifacts to local file system.")
        .DependsOn(Build)
        .After(IntegrationTest)
        .When(IsServerBuild, t => t.DependsOn(IntegrationTest, UnitTest))
        .Executes(() =>
        {
            DotNetTasks.DotNetPublish(s => s
                .SetProject(WebApiProject)
                .SetConfiguration(Configuration)
                .SetOutput(PublicationDirectory)
                .AddProperty("UseAppHost", false)
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target UnitTest => _ => _
        .Description("Evaluates the unit test suite.")
        .DependsOn(Build)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(s => s
                .SetProjectFile(UnitTestsProject)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .SetResultsDirectory(CoverageDirectory)
                .SetDataCollector(DATA_COLLECTOR));
        });

    Target IntegrationTest => _ => _
        .Description("Evaluates the integration test suite.")
        .DependsOn(Build)
        .After(UnitTest)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(s => s
                .SetProjectFile(IntegrationTestsProject)
                .SetConfiguration(Configuration)
                .SetLoggers("console;verbosity=detailed")
                .EnableNoBuild()
                .SetResultsDirectory(CoverageDirectory)
                .SetDataCollector(DATA_COLLECTOR));
        });

    Target Restore => _ => _
        .Description("Restores the NuGet package dependencies for the assemblies in the solution.")
        .DependsOn(Clean)
        .Executes(() => DotNetTasks.DotNetRestore(s => s.SetProjectFile(Solution)));
}
