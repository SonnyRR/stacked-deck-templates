# Stacked Deck Project Templates

A curated collection of C#/.NET project templates, that I've composed for personal use.
Their concept is to give advantage, akin to a "stacked card deck".

## Installation

```sh
dotnet new install StackedDeck.Project.Templates
```

## Usage

```sh
# List the available templates
dotnet new list StackedDeck

# Choose a template & view the required parameters
dotnet new sd-webapi -h

# Update the template
dotnet new update
```

## Available templates

### Stacked Deck Web API (C#)

Sets up a solution with a containerized `ASP.NET Core Web API` service. Comes preconfigured with roslyn analyzers, serilog logging infrastructure
& coding conventions with `.editorconfig`.

#### Features

- Pre-configured `Dockerfile`, targetting `ASP.NET Core Runtime 9` with `alpine` based images.
- Pre-configured `NUKE` build system with common targets for restoring, compiling, publishing, testing & containerizing .NET applications
- Roslyn Analyzers
  - `Roslynator.Analyzers`
  - `SonarAnalyzer.CSharp`
- Structured logging infrastructure with `Serilog`
- Coding conventions (`.editorconfig`)
- Support for `Azure App Configuration`
- Support for `Azure Managed Identity` authentication
- API Versioning
- Automatic dependency version management with `Dependabot`
- Pre-configured `CI` pipeline with `GitHub Actions`
