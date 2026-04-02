using System.Collections.Generic;
using System.Linq;

using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Git;

using Serilog;

namespace Components;

internal interface IVersionArtifacts : IHasGitVersion, IHasGitRepository
{
    static IReadOnlyCollection<Output> Git(ArgumentStringHandler args)
        => GitTasks.Git(args, logger: (_, msg) => Log.Debug(msg));

    [Parameter("The git author email, used for tagging release commits.")]
    string GitAuthorEmail => TryGetValue(() => GitAuthorEmail);

    [Parameter("The git author username, used for tagging release commits.")]
    string GitAuthorUsername => TryGetValue(() => GitAuthorUsername);

    Target TagCommitWithVersion => _ => _
        .Description("Tags the latest commit with a semantic version.")
        .OnlyWhenStatic(() => IsServerBuild)
        .Requires(
            () => GitAuthorEmail,
            () => GitAuthorUsername)
        .TriggeredBy<IDocker>(t => t.PublishImage)
        .Executes(() =>
        {

            Git($"config user.email \"{GitAuthorEmail}\"");
            Git($"config user.name \"{GitAuthorUsername}\"");
            Git($"tag -f -a {SemanticVersion} -m \"Release: '{SemanticVersion}'\"");
            Git($"push origin refs/tags/{SemanticVersion} --force");
        });

    Target CleanupOrphanedGitTags => _ => _
        .Description("Cleans orphaned semantic version git tags.")
        .OnlyWhenStatic(() => IsServerBuild)
        .TriggeredBy(TagCommitWithVersion)
        .Executes(() =>
        {
            var orphanedGitTags = Git(
                    "log --tags='[0-9]*.[0-9]*.[0-9]*-*' --not --remotes=origin --simplify-by-decoration --pretty=format:%D --decorate-refs=refs/tags/[0-9]*.[0-9]*.[0-9]*-*")
                .Select(o => o.Text.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .SelectMany(t => t.Split(", ", System.StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Replace("tag: ", string.Empty))
                .Distinct()
                .ToList();

            if (orphanedGitTags.Count is 0)
            {
                Log.Information("🫧 No orphaned git tags found.");
                return;
            }

            Log.Information("🗑️ Removing {Count} orphaned git tag(s).", orphanedGitTags.Count);
            foreach (var tag in orphanedGitTags)
            {
                Log.Information("🧹 Deleting: {Tag}", tag);
                GitTasks.Git($"push origin --delete refs/tags/{tag}");
            }
        });
}
