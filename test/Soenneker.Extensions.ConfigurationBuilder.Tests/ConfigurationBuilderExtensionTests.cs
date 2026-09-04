using System.Linq;
using Microsoft.Extensions.Configuration.Json;
using Soenneker.Tests.Unit;

namespace Soenneker.Extensions.ConfigurationBuilder.Tests;

public sealed class ConfigurationBuilderExtensionTests : UnitTest
{
    [Test]
    public async System.Threading.Tasks.Task AddAppSettings_canonicalizes_known_environment_casing()
    {
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();

        builder.AddAppSettings("development");

        string? path = builder.Sources.OfType<JsonConfigurationSource>().Single().Path;
        await Assert.That(path).IsEqualTo("appsettings.Development.json");
    }

    [Test]
    public async System.Threading.Tasks.Task AddOcelotConfig_uses_base_file_for_unknown_environment()
    {
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();

        builder.AddOcelotConfig("Preview");

        string? path = builder.Sources.OfType<JsonConfigurationSource>().Single().Path;
        await Assert.That(path).IsEqualTo("ocelot.json");
    }
}
