# Stacked Deck CLI Project Template

A production-ready CLI application template using `Spectre.Console`,
that comes with batteries included and sensible defaults.

TFM: `net10.0`
Identifier: `sd-cli`

View all configuration parameters:

```sh
dotnet new sd-cli -h
```

## 🧩 Features

> [!NOTE]
> Below are mentioned some of the key features of this template. I strongly
> encourage reviewing the source code documents for getting an insight to
> the full suite of features. The project contains in-place implementations
> for command routing, dependency injection, configuration and more.

### 🖥️ Spectre.Console

This template leverages `Spectre.Console` for building beautiful,
cross-platform CLI applications with rich output formatting. The
command-line argument parsing is handled by `Spectre.Console.Cli`,
which supports automatic help generation, argument validation, and
command routing out-of-the-box.

#### 🎯 AsyncCommand Pattern

Commands inherit from `AsyncCommand<T>` where `T` is a settings class
that maps CLI arguments and options to strongly typed properties. This
provides type-safe parameter binding with automatic validation and
description support.

```csharp
public class GreetCommand : AsyncCommand<GreetCommandSettings>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context, GreetCommandSettings settings)
    {
        AnsiConsole.MarkupLine($"[green]Hello, {settings.Name}![/]");
        return 0;
    }
}
```

#### 🏗️ CommandAppBuilder

This template provides a fluent `CommandAppBuilder` API for convenient
initialization and configuration of the `Spectre.Console` application.
The builder handles registration of commands, services, and configuration
providers in a clean, composable manner.

```csharp
public static int Main(string[] args)
    => CommandAppBuilder
        .Create()
        .Build()
        .Run(args);
```

> [!TIP]
> Review the `CommandAppBuilder` class to understand how commands and
> services are wired together. Commands are registered as named subcommands
> (e.g., `config.AddCommand<GreetCommand>("greet")`), making it easy to
> extend the CLI with additional commands.

### ⛓️ Dependency Injection

This template integrates `Microsoft.Extensions.DependencyInjection` with
Spectre.Console's command framework using custom `ITypeRegistrar` and
`ITypeResolver` implementations. All services are registered in the DI
container and resolved automatically for commands and their dependencies.

> [!NOTE]
> The `SpectreCliTypeRegistrar` and `SpectreCliTypeResolver` bridge
> Spectre.Console's internal service resolution with the standard
> Microsoft DI container, enabling seamless integration with the
> broader .NET ecosystem.

### ⚙️ Configuration

Configuration is managed via `Microsoft.Extensions.Configuration` using
environment-specific `appsettings.json` files:

- `appsettings.json` — Base configuration
- `appsettings.Development.json` — Development overrides (`Information` verbosity)
- `appsettings.Staging.json` — Staging overrides (`Information` verbosity)
- `appsettings.Production.json` — Production overrides (`Warning` verbosity)
- `appsettings.Local.json` — Local overrides (gitignored)

The `ConfigurationProvider` class loads and merges the configuration
files in a layered manner, with more specific environments overriding
base values.

```sh
dotnet new sd-cli -n MyCli
```

### 📝 Logging

Logging is provided by `Serilog` with pre-configured sinks:
- **Console** — Colored console output for human-readable logs
- **Async File** — Asynchronous file logging to `logs/log-.log` with rolling file intervals

Logging levels are configured per environment via the `appsettings.*.json`
files, providing fine-grained control over verbosity in different deployment
scenarios.

### 🎨 CLI Parameters

When creating a new project from this template, you can customize the
following parameters:

| Parameter | Default | Description |
|-----------|---------|-------------|
| `--name` / `-n` | `StackedDeckCli` | The project name |
| `--Title` | `StackedDeck CLI Tool` | The CLI tool's display title |
| `--Author` | `Stacked Deck` | The author name displayed in the CLI header |
| `--SkipRestore` | `false` | Skips automatic NuGet restore on create |


### 🧹 Code Formatting

After project creation, `dotnet format` is executed automatically to
ensure consistent code style. The template includes the `EnforceCodeStyleInBuild`
property and generates XML documentation files by default.
