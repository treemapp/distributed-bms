using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DistributedBms.BeckhoffInspector;

public static class InterfaceConfigGenerator
{
    public static string Generate(
        string interfaceName,
        string ipAddress,
        string amsNetId,
        int port,
        string localAmsNetId,
        int version,
        List<BeckhoffSymbol> symbols)
    {
        var config = new InterfaceConfig(
            version,
            interfaceName,
            "beckhoff-ads",
            DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:sszzz"),
            ipAddress,
            amsNetId,
            port,
            localAmsNetId,
            symbols.Count,
            symbols
        );

        var serializer = new SerializerBuilder()
            .WithNamingConvention(
                HyphenatedNamingConvention.Instance
            )
            .Build();

        return serializer.Serialize(config);
    }

    private record InterfaceConfig(
        int Version,
        string Name,
        string Driver,
        string Timestamp,
        string IpAddress,
        string AmsNetId,
        int Port,
        string LocalAmsNetId,
        int NumberOfSources,
        List<BeckhoffSymbol> Sources
    );
}