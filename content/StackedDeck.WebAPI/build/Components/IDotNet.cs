using System.IO;
using System.Linq;

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;

using Serilog;

namespace Components;

internal interface IDotNet : IHasProjects, IHasConfiguration, IHasGitVersion
{
    AbsolutePath PublicationDirectory => WebApiProject.Directory / "publish";

    Target Clean => _ => _
        .Description("Cleans-up the compilation & publication artifacts.")
        .Executes(() =>
        {
            var artifactDirectoriesToDelete = RootDirectory
                .GlobDirectories($"**/{{obj,bin,{Path.GetFileName(PublicationDirectory)}}}")
                .Where(ap => ap.Parent != BuildProjectDirectory);

            foreach (var artifactDir in artifactDirectoriesToDelete)
            {
                Log.Information("🧹 Deleting directory: {ArtifactDirectory}", artifactDir);
                artifactDir.DeleteDirectory();
            }
        });

    Target Build => _ => _
        .Description("Builds the assemblies in the solution.")
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Information("🗂️ Solution File: {Solution}", Solution);
            Log.Information("⚙️ Configuration: {Configuration}", Configuration);
            Log.Information("🎯 Assembly Version: {AssemblySemVer}", GitVersion.AssemblySemVer);
            Log.Information("🎯 File Version: {AssemblySemFileVer}", GitVersion.AssemblySemFileVer);
            Log.Information("🎯 Informational Version: {InformationalVersion}", GitVersion.InformationalVersion);

            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());

            Log.Information("✅ Build completed successfully!");
        });

    Target Publish => _ => _
        .Description("Publishes the API artifacts to local file system.")
        .DependsOn<IDotNet>(t => t.Build)
        .Executes(() =>
        {
            DotNetTasks.DotNetPublish(s => s
                .SetProject(WebApiProject)
                .SetConfiguration(Configuration)
                .AddProperty("UseAppHost", false)
                .EnableNoBuild()
                .SetOutput(PublicationDirectory)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .Description("Evaluates the unit test suite.")
        .DependsOn(Publish)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild());
        });

    Target Restore => _ => _
        .Description("Restores the assemblies' NuGet dependencies")
        .DependsOn(Clean)
        .Executes(() => DotNetTasks.DotNetRestore(s => s.SetProjectFile(Solution)));
}
