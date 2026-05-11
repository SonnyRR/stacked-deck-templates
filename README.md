<!-- markdownlint-disable MD013 MD033-->
<h1 align="center">
  <img src="https://res.cloudinary.com/vasil-kotsev/image/upload/v1760256567/sd-logo-7_ptdcot.png" alt="Stacked Deck" width="256" height="256">
  <br />&nbsp;<br />&nbsp; ♠️ Stacked Deck Project Templates ♠️

[![NuGet Badge](https://img.shields.io/nuget/v/StackedDeck.Project.Templates)](https://www.nuget.org/packages/StackedDeck.Project.Templates/)

</h1>
&nbsp;

A curated collection of opinionated C#/.NET project templates, that I've composed for
personal use, rapid, batteries-included, performant enterprise solutions for
green-field projects. Focusing on sensible defaults, new technologies, enforcable
conventions and low-friction experience OOB.

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
- `OpenTelemetry` observability infrastructure with multiple collection modes
  - `Prometheus` scraping endpoint for metrics-only collection
  - `OTEL Collector` for distributed traces and metrics via `OTLP`
- Infrastructure as Code (`IaC`) directory structure
  - Local development observability stack (`Docker Compose`)
  - `Prometheus`, `Grafana`, `Tempo`, and `OTEL Collector` for local development
- Code coverage collection with `coverlet` & report generation

### 💾 Stacked Deck Persistence (C#)

Provides a class library preconfigured with `Entity Framework Core` for data access
layer implementations using a code-first approach. Supports multiple database providers
with sensible defaults for enterprise applications. Includes built-in auditing and
soft-deletion support out-of-the-box.

#### ✨ Features

- `Entity Framework Core 10` with design-time tooling
- Pre-configured for code-first approach
- Multiple database provider support
  - `SQL Server` (default)
  - `PostgreSQL` with snake_case naming conventions
  - `SQLite` for local development
- Comprehensive auditing support
  - `Audit.NET` integration for cross-provider audit logging (default)
  - Optional temporal tables as alternative for history tracking (MSSQL only)
  - `IAuditableEntity` interface for CreatedBy/CreatedAt/UpdatedBy/UpdatedAt metadata
  - `AuditInterceptor` for automatic audit metadata injection
- Soft delete support via `ISoftDeletableEntity` interface
  - `SoftDeleteInterceptor` converts delete operations to updates automatically
  - Global query filters exclude soft-deleted entities from queries
- Fluent API configuration via `ApplyConfigurationsFromAssembly`
- Extension methods for streamlined service registration
