# Stacked Deck Project Templates

The official `.NET` project templates for **Stacked Deck** services.

The package contains solution templates for containerized `ASP.NET` Web APIs & `Service Fabric` applications.

## Installation

```powershell
# Ensure that you have previously set-up the GitHub NuGet source for FM packages.
dotnet new install StackedDeck.Project.Templates
```

## Usage

```powershell
# List the available templates
dotnet new list StackedDeck

# Choose a template & view the required parameters
dotnet new sd-webapi -h

# Update the template
dotnet new update
```

## Available templates

### Stacked Deck Web API (C#)

Sets up a solution with a containerized `ASP.NET Core Web API` service
Comes preconfigured with roslyn analyzers, serilog logging infrastructure & coding conventions with `.editorconfig`.

#### Features

- Pre-configured `Dockerfile`, targetting `ASP.NET Core Runtime 9` with alpine based images.
- Pre-configured `NUKE` build system with common targets for restoring, compiling, publishing, testing & containerizing .NET applications
- Roslyn Analyzers
  - `Roslynator.Analyzers`
  - `SonarAnalyzer.CSharp`
- Logging infrastructure with `Serilog`
- Coding conventions (`.editorconfig`)
- Pre-configured project structure (Unit tests, Service & Data layers)
- Automatic dependency version management with `Dependabot`
- Pre-configured `CI` pipeline with `GitHub Actions`
- Placeholders for Azure service integrations (`Azure App Configuration` & `Azure Key Vault`)
