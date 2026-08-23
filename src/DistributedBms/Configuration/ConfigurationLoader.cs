using Microsoft.Extensions.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DistributedBms.Configuration;

public class ConfigurationLoader
{
    private readonly string _configDirectory;
    private readonly Dictionary<string, object> _cache = new();
    private readonly IDeserializer _deserializer;

    public ConfigurationLoader(IConfiguration configuration)
    {
        _configDirectory = configuration["ConfigDirectory"]
            ?? throw new InvalidOperationException(
                "ConfigDirectory is not configured"
            );

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public InterfaceConfig GetInterface(string name)
    {
        var cacheKey = $"interfaces/{name}";

        if (_cache.TryGetValue(cacheKey, out var cached))
            return (InterfaceConfig)cached;

        var path = Path.Combine(
            _configDirectory,
            "interfaces",
            $"{name}.yaml"
        );

        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"Configuration file not found: {path}"
            );

        var yaml = File.ReadAllText(path);

        var config = _deserializer.Deserialize<InterfaceConfig>(yaml)
            ?? throw new InvalidOperationException(
                $"Could not deserialize interface configuration: {name}"
            );

        _cache[cacheKey] = config;

        return config;
    }

    public object GetSystem(string name)
    {
        return GetConfig("systems", name);
    }

    private object GetConfig(string directory, string name)
    {
        var cacheKey = $"{directory}/{name}";

        if (_cache.TryGetValue(cacheKey, out var cached))
            return cached;

        var path = Path.Combine(
            _configDirectory,
            directory,
            $"{name}.yaml"
        );

        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"Configuration file not found: {path}"
            );

        var yaml = File.ReadAllText(path);

        var config = _deserializer.Deserialize<object>(yaml)
            ?? throw new InvalidOperationException(
                $"Could not deserialize configuration: {name}"
            );

        _cache[cacheKey] = config;

        return config;
    }
}

