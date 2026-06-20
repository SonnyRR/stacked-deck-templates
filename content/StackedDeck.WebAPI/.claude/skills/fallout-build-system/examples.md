# Quick Reference

Below are examples of how to invoke build targets from the `FALLOUT`
build system.

## 1. Discovering Targets

You can pass the `--help` parameter for a summary of the available build targets
in this codebase:

```sh
fallout --help
```

## 2. Invoking Targets

After you've found an appropriate target, you can invoke it by passing its
name:

```sh
fallout Publish
```

## 3. Passing Parameters to Targets

Some targets require parameters to be passed, you can do that by discovering
the needed parameters and passing them like:

```sh
fallout Build --configuration Release
```
