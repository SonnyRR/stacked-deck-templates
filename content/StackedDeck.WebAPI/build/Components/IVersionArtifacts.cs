using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Git;

using Serilog;

namespace Components;

internal interface IVersionArtifacts : IHasGitVersion, IHasGitRepository
{
    [Parameter("The git author email, used for tagging release commits.")]
    string GitAuthorEmail => TryGetValue(() => GitAuthorEmail);

    [Parameter("The git author username, used for tagging release commits.")]
    string GitAuthorUsername => TryGetValue(() => GitAuthorUsername);

    Target TagCommitWithVersion => _ => _
        .Description("Tags the last commit on a master branch with a semantic version.")
        .OnlyWhenStatic(() => IsServerBuild && GitRepository.IsOnMainOrMasterBranch())
        .Requires(
            () => GitAuthorEmail,
            () => GitAuthorUsername)
        .TriggeredBy<IDocker>(t => t.BuildImage)
        .Executes(() =>
        {
            static void StdOutLogger(OutputType _, string msg) => Log.Debug(msg);
            static void Git(ArgumentStringHandler args) => GitTasks.Git(args, logger: StdOutLogger);

            Git($"config user.email \"{GitAuthorEmail}\"");
            Git($"config user.name \"{GitAuthorUsername}\"");
            Git($"tag -f -a {SemanticVersion} -m \"Release: '{SemanticVersion}'\"");
            Git("push --follow-tags");
        });
}
