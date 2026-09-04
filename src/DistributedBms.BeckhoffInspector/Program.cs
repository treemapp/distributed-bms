using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using TwinCAT.Ads.TcpRouter;

using DistributedBms.BeckhoffInspector;

if (args.Length < 5)
{
    Console.WriteLine(
        "Usage: DistributedBms.BeckhoffInspector " +
        "<interface-name> <ip-address> <ams-net-id> <port> " +
        "<local-ams-net-id> [version] [output-file]"
    );

    return;
}

var interfaceName = args[0];
var ipAddress = args[1];
var amsNetId = args[2];
var localAmsNetId = args[4];

if (!int.TryParse(args[3], out var port))
{
    Console.WriteLine($"Invalid port: {args[3]}");
    return;
}

var version = 1;

if (args.Length >= 6 &&
    !int.TryParse(args[5], out version))
{
    Console.WriteLine($"Invalid version: {args[5]}");
    return;
}

string? outputFile =
    args.Length >= 7
        ? args[6]
        : null;


// ------------------------------------------------------------
// Router
// ------------------------------------------------------------

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


// ------------------------------------------------------------
// ADS connection
// ------------------------------------------------------------

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


// ------------------------------------------------------------
// Read symbols
// ------------------------------------------------------------

var symbolReader = new SymbolReader(client);

Console.WriteLine();
Console.WriteLine("Reading symbols...");
Console.WriteLine();

List<BeckhoffSymbol> symbols;

try
{
    symbols = symbolReader.GetAllSymbols();
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine("***** SYMBOL READING FAILED *****");
    Console.WriteLine(ex);
    return;
}

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


// ------------------------------------------------------------
// Generate interface YAML
// ------------------------------------------------------------

var yaml = InterfaceConfigGenerator.Generate(
    interfaceName,
    ipAddress,
    amsNetId,
    port,
    localAmsNetId,
    version,
    symbols
);


// ------------------------------------------------------------
// Output
// ------------------------------------------------------------

if (string.IsNullOrWhiteSpace(outputFile))
{
    Console.WriteLine();
    Console.WriteLine("Generated interface configuration:");
    Console.WriteLine();
    Console.WriteLine(yaml);
}
else
{
    File.WriteAllText(
        outputFile,
        yaml
    );

    Console.WriteLine();
    Console.WriteLine(
        $"Interface configuration written to: {outputFile}"
    );
}

client.Dispose();