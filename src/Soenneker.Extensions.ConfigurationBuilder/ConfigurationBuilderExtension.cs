using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Soenneker.Enums.DeployEnvironment;
using Soenneker.Extensions.String;

namespace Soenneker.Extensions.ConfigurationBuilder;

/// <summary>
/// Provides extension methods for configuring an <see cref="IConfigurationBuilder"/> with environment-specific app
/// settings, Ocelot configuration files, and environment variables.
/// </summary>
/// <remarks>These extensions help ensure a consistent and predictable configuration source order, particularly in
/// ASP.NET Core applications. They prevent implicit addition of environment-specific JSON files and allow explicit
/// control over which configuration files are loaded based on the specified environment.</remarks>
public static class ConfigurationBuilderExtension
{
    private const bool _defaultOptional = true;
    private const bool _defaultReloadOnChange = false;

    private const string _appSettingsBase = "appsettings.json";
    private const string _appSettingsPrefix = "appsettings.";
    private const string _ocelotBase = "ocelot.json";
    private const string _ocelotPrefix = "ocelot.";

    /// <summary>
    /// Initializes the <see cref="IConfigurationBuilder"/> by removing undesired configuration sources
    /// and adding environment-specific app settings and environment variables.
    /// </summary>
    /// <remarks>
    /// Ensures deterministic configuration order and prevents ASP.NET Core from implicitly
    /// injecting environment-specific JSON files.
    /// </remarks>
    public static IConfigurationBuilder Initialize(this IConfigurationBuilder builder, string? environment)
    {
        IList<IConfigurationSource> sources = builder.Sources;

        // Strip all sources except chained + command-line
        for (int i = sources.Count - 1; i >= 0; i--)
        {
            IConfigurationSource source = sources[i];

            if (source is ChainedConfigurationSource || source is CommandLineConfigurationSource)
                continue;

            sources.RemoveAt(i);
        }

        builder.AddAppSettings(environment);

        // Last so Azure App Settings / env vars win
        builder.AddEnvironmentVariables();

        return builder;
    }

    /// <summary>
    /// Adds the appropriate appsettings JSON file for the specified environment.
    /// </summary>
    public static IConfigurationBuilder AddAppSettings(this IConfigurationBuilder builder, string? environment, bool optional = _defaultOptional,
        bool reloadOnChange = _defaultReloadOnChange)
    {
        builder.AddJsonFile(IsKnownEnvironment(environment) ? BuildEnvJson(_appSettingsPrefix, environment!) : _appSettingsBase, optional, reloadOnChange);

        return builder;
    }

    /// <summary>
    /// Adds the appropriate Ocelot configuration JSON file for the specified environment.
    /// </summary>
    public static IConfigurationBuilder AddOcelotConfig(this IConfigurationBuilder builder, string? environment, bool optional = _defaultOptional,
        bool reloadOnChange = _defaultReloadOnChange)
    {
        builder.AddJsonFile(IsKnownEnvironment(environment) ? BuildEnvJson(_ocelotPrefix, environment!) : _ocelotBase, optional, reloadOnChange);

        return builder;
    }

    private static bool IsKnownEnvironment(string? environment)
    {
        if (environment.IsNullOrEmpty())
            return false;

        return environment.Equals(DeployEnvironment.Production.Name, StringComparison.OrdinalIgnoreCase) ||
               environment.Equals(DeployEnvironment.Staging.Name, StringComparison.OrdinalIgnoreCase) ||
               environment.Equals(DeployEnvironment.Development.Name, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildEnvJson(string prefix, string environment) => string.Concat(prefix, environment, ".json");
}