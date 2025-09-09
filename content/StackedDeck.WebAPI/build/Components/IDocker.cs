using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;

using Serilog;

namespace Components;

internal interface IDocker : IHasProjects, IHasGitVersion
{
    const string DOCKER_IMAGE_NAME = "sd-web-api-image:latest";

    AbsolutePath Dockerfile => WebApiProject.Directory / "Dockerfile";

    Target BuildImage => _ => _
        .Description("Builds an OCI compatible Docker image of the API")
        .DependsOn<IDotNet>(t => t.Test)
        .Executes(() =>
        {
            DockerTasks.DockerBuild(s => s
                .SetPath(WebApiProject.Directory)
                .SetFile(Dockerfile)
                .SetTag(DOCKER_IMAGE_NAME)
                .SetProcessLogger((_, msg) => Log.Debug(msg)));
        });
}
