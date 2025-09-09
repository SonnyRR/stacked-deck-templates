using Nuke.Common.ProjectModel;

namespace Components;

internal interface IHasProjects : IHasSolution
{
    Project WebApiProject => Solution.GetProject("StackedDeck.WebAPI.Template.API");

    Project BuildProject => Solution.GetProject("StackedDeck.WebAPI.Template.Build");
}
