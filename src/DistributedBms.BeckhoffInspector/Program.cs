using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using TwinCAT.Ads.TcpRouter;

using DistributedBms.BeckhoffInspector;

if (args.Length < 4)
{
    Console.WriteLine(
        "Usage: DistributedBms.BeckhoffInspector " +
        "<ip-address> <ams-net-id> <port> <local-ams-net-id>"
    );

    return;
}

var ipAddress = args[0];
var amsNetId = args[1];
var localAmsNetId = args[3];

if (!int.TryParse(args[2], out var port))
{
    Console.WriteLine($"Invalid port: {args[2]}");
    return;
}

// Router
var routerConfiguration =
    new ConfigurationBuilder()
        .AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["AmsRouter:Name"] =
                    "DistributedBms.BeckhoffInspector",

                ["AmsRouter:NetId"] =
                    localAmsNetId,

                ["AmsRouter:RemoteConnections:0:Name"] =
                    "Beckhoff",

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

var router = new AmsTcpIpRouter(
    routerConfiguration,
    loggerFactory
);

Console.WriteLine("Starting ADS router...");

router.StartAsync(
    CancellationToken.None
);//.GetAwaiter().GetResult();

Console.WriteLine("Router started");

var client = new AdsClient();

Console.WriteLine(
    $"Connecting ADS to {amsNetId}:{port}..."
);

try
{
    client.Connect(
        amsNetId,
        port
    );

    Console.WriteLine("ADS connected!");
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine("***** BECKHOFF CONNECTION FAILED *****");
    Console.WriteLine(ex);
    return;
}

var symbolReader = new SymbolReader(client);

symbolReader.TestSymbolLoader();
return;

Console.WriteLine();
Console.WriteLine("Reading symbols...");
Console.WriteLine();

var symbols = symbolReader.GetAllSymbols();

foreach (var symbol in symbols)
{
    Console.WriteLine(
        $"{symbol.Name}\t{symbol.Type}"
    );
}

Console.WriteLine();
Console.WriteLine(
    $"{symbols.Count} symbols found."
);

client.Dispose();