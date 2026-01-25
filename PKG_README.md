<!-- markdownlint-disable MD013-->

# ♠️ Stacked Deck Project Templates

A curated collection of C#/.NET project templates, that I've composed for
personal, for rapid, batteries-included, performant enterprise solutions for
green-field projects. Focusing on sensible defaults, new technologies,
enforcable conventions and low-friction experience OOB.

Their concept is to give advantage, akin to a "stacked card deck".

\- Vasil Kotsev

## 🧰 Installation

```sh
dotnet new install StackedDeck.Project.Templates
```

## 👩‍🔬 Usage

```sh
# List the available templates
dotnet new list StackedDeck

# Choose a template & view the required parameters
dotnet new sd-webapi -h

# Update the template
dotnet new update
```

## 🧩 Available templates

### 🌐 Stacked Deck Web API (C#)

Sets up a solution with a containerized `ASP.NET Core Web API` service.
Comes preconfigured with all the necessary bells & whistles to create
containerized, enterprise-ready, maintainable and performant services
with sensible defaults. For detailed information, see the section below.

#### ✨ Features

- Pre-configured `Dockerfile` with
  - Targeting `ASP.NET Core Runtime 10`
  - `alpine` linux based images
  - Pinned package versions
  - Health checks
- Pre-configured `NUKE` build system with common targets
  - Restoring
  - Compiling
  - Publishing
  - Testing
  - Building `OCI` compatible images
- Semantic versioning with GitVersion
- Roslyn Analyzers
  - `Roslynator.Analyzers`
  - `SonarAnalyzer.CSharp`
- Structured logging infrastructure with `Serilog`
- Coding conventions (`.editorconfig`)
- Multiple API styles with full API versioning support
  - `Controllers`
  - `Minimal API (default)`
  - `FastEndpoints`
- `OpenAPI v3` specification generation
- `Scalar` API client for interacting with the `OpenAPI` specification
- Optional support for `Azure App Configuration`
- Optional support for `Azure Managed Identity` authentication
- Health checks endpoint with liveness probe and system metadata
- Strongly typed configurations via dedicated environment-specific `appsettings.json` documents
- Shared project properties via `Directory.Build.props`
- Automatic dependency version management with `Dependabot`
- Optional pre-configured `CI` pipeline with `GitHub Actions`
- AI Agent Skills for NUKE build system and git operations
