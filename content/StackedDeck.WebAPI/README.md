# Stacked Deck ASP.NET Web API Solution Template

A production-ready `ASP.NET Core Web API` solution template, that comes with
batteries included and sensible defaults.

> [!WARNING]
> This template is currently in a stable state, but some of it's features
> are more opinionated towards the Azure & GitHub ecosystems. Support for
> `Azure App Config`, `GitHub Workflows` & `Dependabot` comes OOB with the
> current version. This might not be suitable for everyone & multiple
> options for cloud & `CI/CD` providers will be provided. As of right now
> you will have to remove those documents manually if you don't wish to
> use `GitHub` as your pipeline provider.

TFM: `net9.0`
Identifier: `sd-webapi`

View all configuration parameters:

```sh
dotnet new sd-webapi -h
```

## 🧩 Features

> [!NOTE]
> Below are mentioned some of the key features of this template. I strongly
> encourage reviewing the source code documents for getting an insight to
> the full suite of features. The solution contains in-place implementations
> for global error handling, semantic versioning, security policies, OpenAPI
> specification, strongly typed options, integration test fixtures and more.

### 🏗️ NUKE build system

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

### 🐳 Docker support

This template comes with a pre-configured `Dockerfile`, utilizing `aspnet:9.0-alpine`
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

### ✍🏻 Structured logging

This project leverages [Serilog](https://github.com/serilog/serilog) with a custom
configuration that allows for structured logs to be sent to the `STDOUT` & `STDERR`
output streams. You can ingest them via tools like [Grafana Alloy](https://grafana.com/docs/alloy/latest/)
or set-up additional sinks for structured logs servers like [Seq](https://datalust.co/).

### 🪆 API Versioning

API Versioning is supported out-of-the-box with this template. Allowing for hosting
multiple versions of endpoints or controller in the same host to ensure backwards
compatibility.

### 🗺️ OpenAPI specification

An `OpenAPI v3` specification is automatically generated at runtime via the
`Microsoft.AspNetCore.OpenApi` package with this template.

### 🕵🏻‍♂️ Scalar

[Scalar](https://scalar.com/) is the UI of choice for interacting with the `OpenAPI`
specification. It is a fully-fledged interactive API client inside of your browser
and trusted by Fortune 500 companies. Heavily customizable, you can extend to
comply with your company's design guidelines & security practices.

### ⚙️ Azure App Configuration

This template can utilize retrieving, registering & validating variables and secrets
from `Azure App Config` instances for all environments that are not `LOCAL`.
Authentication is setup with `User Managed Identities`, so that you can minimize
the storage of sensitive data in `appsettings.*.json` documents.

### 🔁 CI/CD GitHub Workflow

This template uses a configuration for `GitHub Actions`, which builds & pushes `OCI`
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

### 🤖 Dependabot

`Dependabot` will automatically track the dependency tree of this repository and
open PRs whenever a new `NuGet` pkg version is released or an updated `Dockerfile`
dependency is pushed. It will also notify you of security incidents and vulnerabilities
related to your 3rd party dependencies.
