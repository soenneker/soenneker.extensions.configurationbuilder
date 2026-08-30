[![](https://img.shields.io/nuget/v/soenneker.extensions.configurationbuilder.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.configurationbuilder/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.configurationbuilder/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.configurationbuilder/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.configurationbuilder.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.configurationbuilder/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.configurationbuilder/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.configurationbuilder/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.ConfigurationBuilder

Adds predictable appsettings, Ocelot, and environment-variable sources to an `IConfigurationBuilder`.

## Installation

```bash
dotnet add package Soenneker.Extensions.ConfigurationBuilder
```

## Initialize a builder

```csharp
using Soenneker.Extensions.ConfigurationBuilder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.Initialize(builder.Environment.EnvironmentName);
```

`Initialize()` deliberately rebuilds most of the source list. It:

1. Keeps existing chained and command-line sources and removes all other sources.
2. Adds one appsettings file for the supplied environment.
3. Adds unprefixed environment variables last, so they override the retained sources and JSON values.

Call it before adding any custom configuration providers that must be retained. Command-line sources are preserved, but environment variables added by this method have higher precedence.

## File selection

The recognized environments and selected files are:

| Environment | App settings | Ocelot |
| --- | --- | --- |
| `Development` | `appsettings.Development.json` | `ocelot.Development.json` |
| `Staging` | `appsettings.Staging.json` | `ocelot.Staging.json` |
| `Production` | `appsettings.Production.json` | `ocelot.Production.json` |
| Missing or any other value | `appsettings.json` | `ocelot.json` |

Environment matching is case-insensitive, while generated filenames use the canonical casing shown above. Each method adds exactly one JSON file; it does not layer the base file beneath an environment-specific file.

JSON files are optional and do not reload by default. Override either behavior when adding a source directly:

```csharp
builder.Configuration.AddAppSettings(
    builder.Environment.EnvironmentName,
    optional: false,
    reloadOnChange: true);

builder.Configuration.AddOcelotConfig(
    builder.Environment.EnvironmentName,
    optional: false);
```

`AddAppSettings()` and `AddOcelotConfig()` append their JSON source without removing existing providers. Use those methods instead of `Initialize()` when you need to keep the builder's existing source collection.
