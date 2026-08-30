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
    /// Removes every source except chained and command-line sources, then adds one appsettings JSON source and environment variables.
    /// </summary>
    /// <remarks>
    /// Environment variables are added last and therefore take precedence. Custom providers that must be retained should be added after this method.
    /// </remarks>
    /// <param name="builder">The builder to reconfigure.</param>
    /// <param name="environment">The deployment environment used to select the appsettings file.</param>
    /// <returns>The same builder instance.</returns>
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
    /// Adds one appsettings JSON file for the specified environment.
    /// </summary>
    /// <param name="builder">The builder to which the JSON source is added.</param>
    /// <param name="environment">The deployment environment used to select the filename.</param>
    /// <param name="optional">Whether a missing file is allowed.</param>
    /// <param name="reloadOnChange">Whether the source reloads when the file changes.</param>
    /// <returns>The same builder instance.</returns>
    public static IConfigurationBuilder AddAppSettings(this IConfigurationBuilder builder, string? environment, bool optional = _defaultOptional,
        bool reloadOnChange = _defaultReloadOnChange)
    {
        string path = TryGetKnownEnvironmentName(environment, out string? knownEnvironment)
            ? BuildEnvJson(_appSettingsPrefix, knownEnvironment)
            : _appSettingsBase;

        builder.AddJsonFile(path, optional, reloadOnChange);

        return builder;
    }

    /// <summary>
    /// Adds one Ocelot JSON file for the specified environment.
    /// </summary>
    /// <param name="builder">The builder to which the JSON source is added.</param>
    /// <param name="environment">The deployment environment used to select the filename.</param>
    /// <param name="optional">Whether a missing file is allowed.</param>
    /// <param name="reloadOnChange">Whether the source reloads when the file changes.</param>
    /// <returns>The same builder instance.</returns>
    public static IConfigurationBuilder AddOcelotConfig(this IConfigurationBuilder builder, string? environment, bool optional = _defaultOptional,
        bool reloadOnChange = _defaultReloadOnChange)
    {
        string path = TryGetKnownEnvironmentName(environment, out string? knownEnvironment)
            ? BuildEnvJson(_ocelotPrefix, knownEnvironment)
            : _ocelotBase;

        builder.AddJsonFile(path, optional, reloadOnChange);

        return builder;
    }

    private static bool TryGetKnownEnvironmentName(string? environment, out string knownEnvironment)
    {
        if (environment.IsNullOrEmpty())
        {
            knownEnvironment = null!;
            return false;
        }

        if (environment.Equals(DeployEnvironment.Production.Name, StringComparison.OrdinalIgnoreCase))
            knownEnvironment = DeployEnvironment.Production.Name;
        else if (environment.Equals(DeployEnvironment.Staging.Name, StringComparison.OrdinalIgnoreCase))
            knownEnvironment = DeployEnvironment.Staging.Name;
        else if (environment.Equals(DeployEnvironment.Development.Name, StringComparison.OrdinalIgnoreCase))
            knownEnvironment = DeployEnvironment.Development.Name;
        else
        {
            knownEnvironment = null!;
            return false;
        }

        return true;
    }

    private static string BuildEnvJson(string prefix, string environment) => string.Concat(prefix, environment, ".json");
}
