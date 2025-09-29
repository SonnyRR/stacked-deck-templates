# Stacked Deck ASP.NET Web API Template

## 🛠️ Prerequisites

## 🚀 Running the Nuke Build

This project uses **Nuke** as the build automation tool. Follow the steps below to install and execute the build process.

### Prerequisites

Ensure you have the following installed on your system:

- **.NET SDK 9+** (Required for executing Nuke build scripts)
- **Git** (For version control and GitVersion)

### One-Time Setup

If you haven’t installed **Nuke**, run the following command to bootstrap it:

```sh
dotnet tool install --global Nuke.GlobalTool
```

### Running the build

To list the available **Nuke** targets and their parameters, run:

```sh
nuke --help
```

To execute the default build pipeline, run:

```sh
nuke
```

You can run specific build targets using:

```sh
nuke <Target>
```

## 🐞 Remote container debugging

Remote debugging the application as a container requires some additional steps. Ensure that you're in the API assembly directory and follow these steps:

### Spin-up the application container

```sh
# Publish artifacts under DEBUG configuration
nuke publish --configuration Debug

# Build the image
docker buildx build -t {your-img-name}:debug .

# Spin up the container
docker run -p 5133:8080 --name {your-img-name}-debug --rm {your-img-name}:debug
```

### Remote attach a debugger from VS

https://learn.microsoft.com/en-us/visualstudio/debugger/attach-to-process-running-in-docker-container
