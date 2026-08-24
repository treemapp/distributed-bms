/*
ConfigurationLoader
└── _cache
    ├── "interfaces/simulator"
    │     └── Dictionary<string, object>
    │           ├── version
    │           ├── name
    │           ├── driver
    │           ├── scan-interval-ms
    │           └── sources
    │
    └── "systems/building-1"
          └── Dictionary<string, object>
                └── ...
*/
using Microsoft.Extensions.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DistributedBms.Configuration;

public class ConfigurationLoader
{
    private readonly string _configDirectory;
    private readonly Dictionary<string, object?> _cache = new();
    private readonly IDeserializer _deserializer;

    public ConfigurationLoader(IConfiguration configuration)
    {
        _configDirectory = configuration["ConfigDirectory"]
            ?? throw new InvalidOperationException(
                "ConfigDirectory is not configured"
            );

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();
    }

    public Dictionary<string, object?> GetInterface(string name)
    {
        var config = GetConfig("interfaces", name);

        if (!config.ContainsKey("name") ||
            !config.ContainsKey("driver"))
        {
            throw new InvalidOperationException(
                $"Missing required config fields in {name}.yaml"
            );
        }

        return config;
    }

    public Dictionary<string, object?> GetSystem(string name)
    {
        return GetConfig("systems", name);
    }

    private Dictionary<string, object?> GetConfig(
        string directory,
        string name)
    {
        var cacheKey = $"{directory}/{name}";

        if (_cache.TryGetValue(cacheKey, out var cached) &&
            cached is Dictionary<string, object?> cachedConfig)
        {
            return cachedConfig;
        }

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

        Dictionary<string, object?> config = _deserializer
            .Deserialize<Dictionary<string, object?>>(yaml)
            ?? throw new InvalidOperationException(
                $"Could not deserialize configuration: {name}"
            );

        _cache[cacheKey] = config;

        return config;
    }
}