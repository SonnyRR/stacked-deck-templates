<h1 align="center">
  <img src="https://res.cloudinary.com/vasil-kotsev/image/upload/v1760256567/sd-logo-7_ptdcot.png" alt="Stacked Deck" width="256" height="256">
  <br />&nbsp;<br />&nbsp; ♠️ Stacked Deck Project Templates ♠️

[![NuGet Badge](https://img.shields.io/nuget/v/StackedDeck.Project.Templates)](https://www.nuget.org/packages/StackedDeck.Project.Templates/)

</h1>
&nbsp;

A curated collection of C#/.NET project templates, that I've composed for
personal use. Their concept is to give advantage, akin to a "stacked card deck".

> [!WARNING]
> While this package is stable and production-ready, there are still some
> things that are 🚧 WIP. This warning will be removed when the first stable
> version is published into the offical NuGet gallery.

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
Comes preconfigured with roslyn analyzers, serilog logging infrastructure
& coding conventions with `.editorconfig`.

#### ✨ Features

- Pre-configured `Dockerfile`, targetting `ASP.NET Core Runtime 10` with
  `alpine` based images.
- Pre-configured `NUKE` build system with common targets for restoring,
  compiling, publishing, testing & containerizing .NET applications
- Roslyn Analyzers
  - `Roslynator.Analyzers`
  - `SonarAnalyzer.CSharp`
- Structured logging infrastructure with `Serilog`
- Coding conventions (`.editorconfig`)
- API Versioning
- `OpenAPI v3` specification generation
- `Scalar` API client for interacting with the `OpenAPI` specification
- Support for `Azure App Configuration`
- Support for `Azure Managed Identity` authentication
- Automatic dependency version management with `Dependabot`
- Pre-configured `CI` pipeline with `GitHub Actions`
