using System;

using Nuke.Common;

using StackedDeck.WebAPI.Template.Build;

namespace Components;

internal interface IHasConfiguration : INukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    Configuration Configuration => TryGetValue(() => Configuration) ?? (IsLocalBuild ? Configuration.Debug : Configuration.Release);

    HostEnvironment HostEnvironment => Enum.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), out HostEnvironment environment)
        ? environment
        : HostEnvironment.Local;
}
