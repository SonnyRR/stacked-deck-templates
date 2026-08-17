using System;
using System.Linq;

using Fallout.Solutions;

namespace Components;

internal interface IHasProjects : IHasSolution
{
    Project WebApiProject => Solution.GetProject("StackedDeck.WebAPI.Template.API");

    Project BuildProject => Solution.GetProject("StackedDeck.WebAPI.Template.Build");

    Project UnitTestsProject => Solution
        .AllProjects
        .Single(p => string.Equals(p.Name, "StackedDeck.WebAPI.Template.Unit.Tests", StringComparison.Ordinal));

    Project IntegrationTestsProject => Solution
        .AllProjects
        .Single(p => string.Equals(p.Name, "StackedDeck.WebAPI.Template.Integration.Tests", StringComparison.Ordinal));
}
