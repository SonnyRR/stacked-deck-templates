using System;

using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;

using Polly;

using Serilog;

using static Nuke.Common.Tools.Docker.DockerTasks;

namespace Components;

internal interface IDocker : IHasProjects, IHasGitVersion, IHasGitRepository
{
    const string IMAGE_NAME = "sd-web-api-image";
    const string LATEST_IMAGE_TAG = "latest";
    const char IMAGE_TAG_DELIMITER = ':';

    [Secret]
    [Parameter("The PAT used in order to push the OCI compatible image to the container registry.")]
    string ContainerRegistryPAT => TryGetValue(() => ContainerRegistryPAT);

    [Parameter("The username used to login into the container registry.")]
    string ContainerRegistryUsername => TryGetValue(() => ContainerRegistryUsername);

    [Parameter("The container registry URL.")]
    string ContainerRegistryUrl => TryGetValue(() => ContainerRegistryUrl);

    AbsolutePath Dockerfile => WebApiProject.Directory / "Dockerfile";

    private string LatestImageTag => IMAGE_NAME + IMAGE_TAG_DELIMITER + LATEST_IMAGE_TAG;
    private string SemVerImageTag => IMAGE_NAME + IMAGE_TAG_DELIMITER + SemanticVersion;

    Target BuildImage => _ => _
        .Description("Builds an OCI compatible image of the API.")
        .DependsOn<IDotNet>(t => t.Publish)
        .Executes(() =>
        {
            DockerBuild(s => s
                .SetPath(WebApiProject.Directory)
                .SetFile(Dockerfile)
                .SetTag(LatestImageTag)
                .SetProcessLogger((_, msg) => Log.Debug(msg)));
        });

    Target PublishImage => _ => _
        .Description("Publishes an OCI compatible image to a provided container registry.")
        .DependsOn(BuildImage)
        .Requires(
            () => ContainerRegistryUrl,
            () => ContainerRegistryPAT,
            () => ContainerRegistryUsername)
        .Executes(() =>
        {
            Policy
                .Handle<Exception>()
                .WaitAndRetry(5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, _, retryCount, _) =>
                    {
                        Log.Warning(ex, "Docker login was unsuccessful");
                        Log.Information("Attempting to login into GitHub Docker image registry. Try #{RetryCount}", retryCount);
                    })
                .Execute(() => DockerLogin(s => s
                    .SetServer(ContainerRegistryUrl)
                    .SetUsername(ContainerRegistryUsername)
                    .SetPassword(ContainerRegistryPAT)
                    .DisableProcessOutputLogging()));

            var SemanticallyVersionedImageName = $"{ContainerRegistryUrl}/{SemVerImageTag}";
            DockerTag(s => s.SetSourceImage(LatestImageTag).SetTargetImage(SemanticallyVersionedImageName));
            DockerPush(s => s.SetName(SemanticallyVersionedImageName));

            if (GitRepository.IsOnMainOrMasterBranch())
            {
                var LatestImageName = $"{ContainerRegistryUrl}/{LatestImageTag}";
                DockerTag(s => s.SetSourceImage(LatestImageTag).SetTargetImage(LatestImageName));
                DockerPush(s => s.SetName(LatestImageName));
            }
        });
}
