# sd-api-title

This project was created using the [StackedDeck.WebAPI](templateUrl) version `vtemplateVersion`

## 🧰 Prerequisites

Ensure the following tools, SDKs & runtimes are installed in order to compile
this project.

- `git`
- `docker`
- `dotnet SDK` - can be installed automatically by the `NUKE` build system,
  via the bootstrap scripts.

## 🚀 Quickstart

### 🏗️ Build system

This project uses the [NUKE](https://nuke.build/) build system to manage common
`CI/CD` and `custom` tasks, that are needed during the development lifecycle of
this application. A dedicated build system allows for the abstracting of common
`CI/CD` tasks in a `DSL`, in this case `C#`. The benefits are that you can debug
those tasks, have slim `*.yml` documents for pipeline configurations and reduce
dependencies on 3rd party pipeline providers & their environments.

By default it comes with 3 bootstrap scripts in the root directory:

- `build.sh`
- `build.ps1`
- `build.cmd`

These scripts are primarily intended to be used as the entrypoints `*.yml` documents
for various `CI/CD` providers, like `GitHub Actions`, `Azure Pipelines` and others.
But you can also invoke them on your own.

> [!TIP]
> It's recommended that you install the global dotnet tool for `NUKE`, so that
> you can skip specifying the build script path before invoking a dedicated target.

```sh
dotnet tool install Nuke.GlobalTool --global
```

#### 🎯 Targets

`Target` is a small job or a task, that can be part of the `CI/CD` process or something
entirely custom, like managing `EF Core` migrations or generating `SBOM`.

In order to get started, you will need to see what are the available targets:

```sh
# You can use the aforementioned boostrap scripts.
./build.sh --help

# Or if you installed the NUKE global tool
nuke --help
```

To run a target you will need to specify it's identifier:

```sh
./build.sh BuildImage

# Or through the NUKE global tool
nuke BuildImage
```

By default, the `Build` target will be executed if you don't provide an explicit
target name.

> [!TIP]
> The target implementations can be found under the `Components` directory inside
> the build assembly. New targets can be added depending on the needs and requirements.

## 🔁 CI/CD providers

This solution uses a configuration for `GitHub Actions`, which builds & pushes `OCI`
compatible images to a container registry of your choice. It also makes use of
`dependabot` for automatic dependency version management. Both configurations are
specific to `GitHub` services, if you're hosting your pipelines on other
providers - adjust the configurations accordingly. Ensure that the `yaml` documents
are slim, they should only be concerned with bootsrapping the `NUKE` scripts & to
pass forward the custom parameters.

> [!WARNING]
> Before trying to trigger a workflow run, review the `.github/workflows/ci.yml`
> document and the comments inside of it. You are required to setup repository
> variables & secrets before initiating workflow runs.

## 🧩 Features

### 🐳 Docker support

This project comes with a pre-configured `Dockerfile`, utilizing `aspnet:10.0-alpine`
as the base runtime image. Globalization features are enabled through the
`International Components for Unicode` packages from the `alpine` package registry.

To build an `OCI` compatible image, you need to run the following `NUKE` target:

```sh
nuke BuildImage
```

You can also publish images to container registries:

```sh
nuke PublishImage \
  --container-registry-pat '{PAT}' \
  --container-registry-url '{URL}' \
  --container-registry-username '{USERNAME}'
```

### 📕 Coding standards & conventions

A pre-configured [.editorconfig](https://editorconfig.org/) document with sensible
defaults ensures that coding standards must be followed regardless of the contributor
to this codebase.

### 🔎 Static code analysis

Static code analysis is performed via the `Roslyn` compiler platform and
extended with the `Roslynator.Analyzers` & `SonarAnalyzer.CSharp` analyzers,
to ensure code quality & standards enforced by the `.editorconfig` preferences.

### 🏗️ NUKE build system

As mentioned in the quickstart section, this project utilizes a dedicated build
system - [NUKE](https://github.com/nuke-build/nuke), which allows for abstacting
& automating common build/deploy tasks in a domain specific language - C#.
Having all of your `CI/CD` tasks wrapped in C# code allows for easier debugging,
reproducibility across multiple environments, more pleasant maintenance & decoupling
from specific pipeline providers like `GitHub Actions` or `Azure DevOps` pipelines.

### ✍🏻 Structured logging

This project leverages [Serilog](https://github.com/serilog/serilog) with a custom
configuration that allows for structured logs to be sent to the `STDOUT` & `STDERR`
output streams. You can ingest them via tools like [Grafana Alloy](https://grafana.com/docs/alloy/latest/)
or set-up additional sinks for structured logs servers like [Seq](https://datalust.co/).

### 🪆 API Versioning

API Versioning is supported out-of-the-box, so if this project matures in time, you
can easily host multiple versions inside of this web service.

### 🗺️ OpenAPI specification

An `OpenAPI v3` specification is automatically generated at runtime via the
`Microsoft.AspNetCore.OpenApi` package.

### 🕵🏻‍♂️ Scalar

[Scalar](https://scalar.com/) is the UI of choice for interacting with the `OpenAPI`
specification. It is a fully-fledged interactive API client inside of your browser
and trusted by Fortune 500 companies. Heavily customizable, you can extend to
comply with your company's design guidelines & security practices.

### ⚙️ Azure App Configuration

This project can utilize retrieving, registering & validating variables and secrets
from `Azure App Config` instances for all environments that are not `LOCAL`.
Authentication is setup with `User Managed Identities`, so that you can minimize
the storage of sensitive data in `appsettings.*.json` documents.

### 🔁 GitHub Actions Workflow

By default this project comes with a pre-defined `CI` workflow for `GitHub Actions`.
It builds an `OSI` compatible container image and pushes it to a chosen container
registry, that you must configure before running the pipeline.

### 🤖 Dependabot

`Dependabot` will automatically track the dependency tree of this repository and
open PRs whenever a new `NuGet` pkg version is released or an updated `Dockerfile`
dependency is pushed. It will also notify you of security incidents and vulnerabilities
related to your 3rd party dependencies.
