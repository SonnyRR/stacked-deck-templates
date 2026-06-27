<!-- markdownlint-disable MD013 MD024-->

# ♠️ Stacked Deck Project Templates

A curated collection of opinionated C#/.NET project templates, that I've composed for
personal, for rapid, batteries-included, performant enterprise solutions for
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

### ⌨️ Stacked Deck CLI (C#)

Provides a CLI application template preconfigured with `Spectre.Console`
for building beautiful, cross-platform command-line tools. Integrates
dependency injection, logging, and environment-specific
configuration out-of-the-box.

#### ✨ Features

- `Spectre.Console` for rich, cross-platform CLI output
- `Spectre.Console.Cli` for command parsing and routing
  - `AsyncCommand<T>` pattern for type-safe parameter binding
  - Named subcommand registration (e.g., `config.AddCommand<GreetCommand>("greet")`)
- Fluent `CommandAppBuilder` API for clean application setup
- `Microsoft.Extensions.DependencyInjection` integration via custom type registrar and resolver
- `Microsoft.Extensions.Configuration` with environment-specific `appsettings.json` files
  - Development, Staging, Production, and Local configuration layers
- `Serilog` logging with console and async file sinks
- Configurable logging levels per environment
- Customizable CLI title and author display name
- `dotnet format` post-action for consistent code style
- XML documentation generation enabled by default
