[![](https://img.shields.io/nuget/v/soenneker.extensions.configurationbuilder.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.configurationbuilder/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.configurationbuilder/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.configurationbuilder/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.configurationbuilder.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.configurationbuilder/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.configurationbuilder/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.configurationbuilder/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.ConfigurationBuilder

A collection of helpful ConfigurationBuilder extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.ConfigurationBuilder
```

## Quick start

```csharp
using Soenneker.Extensions.ConfigurationBuilder;

// Given an existing IConfigurationBuilder named builder:
var result = builder.Initialize(environment);
```

## Common operations

- `Initialize()` - Initializes the `IConfigurationBuilder` by removing undesired configuration sources and adding environment-specific app settings and environment variables.
- `AddAppSettings()` - Adds the appropriate appsettings JSON file for the specified environment.
- `AddOcelotConfig()` - Adds the appropriate Ocelot configuration JSON file for the specified environment.
