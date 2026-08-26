using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using TwinCAT.Ads.TcpRouter;

namespace DistributedBms.Drivers;

public class BeckhoffDriver : IPlcDriver
{
    private readonly AdsClient _client;
    private readonly AmsTcpIpRouter _router;

    private readonly Dictionary<string, string> _points = new();

    public BeckhoffDriver(
        Dictionary<string, object?> config)
    {
        var name = GetRequiredString(
            config,
            "name"
        );

        var amsNetId = GetRequiredString(
            config,
            "ams-net-id"
        );

        var ipAddress = GetRequiredString(
            config,
            "ip-address"
        );

        var port = GetRequiredInt(
            config,
            "port"
        );

        var localAmsNetId = GetRequiredString(
            config,
            "local-ams-net-id"
        );

        if (!config.TryGetValue("sources", out var sources) ||
            sources is not IEnumerable<object> sourceList)
        {
            throw new InvalidOperationException(
                "Beckhoff configuration is missing 'sources'"
            );
        }

        foreach (var source in sourceList)
        {
            if (source is not Dictionary<object, object?> sourceConfig)
            {
                throw new InvalidOperationException(
                    "Invalid Beckhoff source configuration"
                );
            }

            if (!sourceConfig.TryGetValue(
                    "id",
                    out var idValue) ||
                idValue is null)
            {
                throw new InvalidOperationException(
                    "Beckhoff source is missing 'id'"
                );
            }

            var id = sourceConfig["id"]?.ToString()
                ?? throw new InvalidOperationException(
                    "Beckhoff source is missing 'id'"
                );

            var symbol = sourceConfig["symbol"]?.ToString()
                ?? throw new InvalidOperationException(
                    $"Beckhoff source '{id}' is missing 'symbol'"
            );

            _points[id] = symbol;
        }

        /*
         * Create the ADS router configuration in memory.
         *
         * The router expects the same configuration structure
         * that we previously supplied through appsettings.json.
         */
        var routerConfiguration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["AmsRouter:Name"] =
                            "DistributedBms",

                        ["AmsRouter:NetId"] =
                            localAmsNetId,

                        ["AmsRouter:RemoteConnections:0:Name"] =
                            name,

                        ["AmsRouter:RemoteConnections:0:Address"] =
                            ipAddress,

                        ["AmsRouter:RemoteConnections:0:NetId"] =
                            amsNetId,

                        ["AmsRouter:RemoteConnections:0:Type"] =
                            "TCP_IP"
                    })
                .Build();

        var loggerFactory = LoggerFactory.Create(
            logging =>
            {
                logging.AddConsole();
            });

        _router = new AmsTcpIpRouter(
            routerConfiguration,
            loggerFactory
        );

Console.WriteLine("Router started");

        _router.StartAsync(
            CancellationToken.None
        );//.GetAwaiter().GetResult();
Console.WriteLine("Async started");
        _client = new AdsClient();
Console.WriteLine("Client created");
try
{
    Console.WriteLine(
        $"Connecting ADS to {amsNetId}:{port}..."
    );
        _client.Connect(
            amsNetId,
            port
        );
    Console.WriteLine(
        $"Beckhoff ADS connected: {amsNetId}:{port}"
    );
}
catch (Exception ex)
{
    Console.WriteLine(
        $"***** BECKHOFF CONNECT FAILED *****"
    );

    Console.WriteLine(ex.ToString());

    throw;
}
        Console.WriteLine(
            $"Beckhoff ADS connected: {amsNetId}:{port}"
        );
    }

    public Task<object> ReadAsync()
    {
        var result = new Dictionary<string, object?>();

        foreach (var point in _points)
        {
Console.WriteLine($"ADS READ: {point.Key} -> {point.Value}");

            var value = _client.ReadValue(
                point.Value,
                typeof(float)
            );

Console.WriteLine($"ADS RESULT: {point.Key} = {value}");

            result[point.Key] = value;
        }

        return Task.FromResult<object>(result);
    }

    public Task WriteAsync(
	    //string interfaceName,
        string sourceId,
        object value)
    {
        if (!_points.TryGetValue(sourceId, out var symbol))
        {
            throw new InvalidOperationException(
                $"Unknown Beckhoff source: {sourceId}"
            );
        }

        var typedValue =  ((JsonElement)value).GetSingle();

        Console.WriteLine(
            $"ADS WRITE: {sourceId} -> {symbol} = {typedValue}"
        );

        _client.WriteValue(
            symbol,
            typedValue
        );

        return Task.CompletedTask;
    }

    private static string GetRequiredString(
        Dictionary<string, object?> config,
        string key)
    {
        return config[key]?.ToString()
            ?? throw new InvalidOperationException(
                $"Beckhoff configuration is missing '{key}'"
            );
    }

    private static int GetRequiredInt(
        Dictionary<string, object?> config,
        string key)
    {
        if (!config.TryGetValue(key, out var value) ||
            value is null)
        {
            throw new InvalidOperationException(
                $"Beckhoff configuration is missing '{key}'"
            );
        }

        return Convert.ToInt32(value);
    }

    private static string GetLocalAmsNetId(
        string ipAddress)
    {
        return $"{ipAddress}.1.1";
    }
}