# Agent Development Guide

Below are instructions for you, the AI agent contributing to this codebase.
Follow them closely & respect them.

## General

- Main source code is located under `./src/`.
- Test assemblies should be maintained under the `./test/`.
- All build system infrastructure is located in `./build/`.
- All configurations must be done via `appsettings.{ENV}.json`
- Ensure that `ASPNETCORE_ENVIRONMENT` env. variable is set to `Local`
  when working on this codebase, unless explicitly prompted by the user
  to use a different environment.
  documents. They are located in `./src/StackedDeck.WebAPI.Template.API/`
- Mimic the project structure in other assemblies.
- Prefer loosely coupled implementations with strong cohesion.
- Ensure new code is covered by unit & integration tests, where
  applicable.
- Prefer small changes, avoid `big-bang` suggestions or refactorings
  unless explicitly prompted to do so.
- Ensure that the affected assembly(ies) are compiled successfully
  after any changes were applied to you to source code documents. This
  should be your final step in your plans.
- Prefer a more functional programming approach where appropriate.

## Coding Guide

- Obey the rulesets defined in `./.editorconfig` when writing code.
- Ensure that your code complies with suggestions from all `Roslyn`
  static code analyzers.
- Add comprehensive and meaningful `XML` documentation for new members.
- Mimic the style of the existing codebase.
- When writing `LINQ` statements prefer the extension methods for `IEnumerable<T>`
  and `IQueryable<T>`. Always split them on new lines.
- Always add `CancellationToken` values where appropriate. Assign a default one
  for new method implementations.
- Try to avoid `async` state machines where deemed appropriate, suggest to return
  `Task` instances directly where it makes sense. Don't be too agressive about
  it though.
- Use pattern matching as much as possible.
- Inject & use `ILogger<T>` where appropriate, write concise and meaningful log
  statements. Don't be afraid to use different verbosity levels.

## Testing Guide

- Always delimit test scenarios with `Arrange`, `Act` `Assert` comments
  where approprate. Some tests scenarios can combine them (e.g., testing exceptions)
- When annotating scenarios with the `xUnit` attributes, always assign a human
  readable display name for the test.
  Example: `[Fact(DisplayName="A brief description of  the scenario")]`
- For test method identifiers use the following convention: `{SUT}_{Scenario}_{Expectation}`
  Example: `Add_MaximumSumResult_ThrowsOverflowException`
- For integration tests, prefer using fixtures as much as possible.
- Integration tests should always use a single global fixture for spinning-up
  an instance of the `API`.
- Prefer using the `dotnet CLI` to filter and run single test or a subset of tests
  to verify your changes. After successful pass, evaluate all tests via the
  `NUKE` targets
