using System.Buffers.Binary;
using System.Text;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;

namespace DistributedBms.BeckhoffInspector;


public class SymbolReader
{
    private const uint AdsGrpSymUploadInfo2 = 0xF00F;
    private const uint AdsGrpSymUpload = 0xF00B;
    private const uint AdsOffsDevDataAdsState = 0x0000;

    private readonly AdsClient _client;

    public SymbolReader(AdsClient client)
    {
        _client = client;
    }

    public List<BeckhoffSymbol> GetAllSymbols()
    {
        //var infoBuffer = new byte[8];

        var infoResult = _client.ReadAsResult(
            AdsGrpSymUploadInfo2,
            AdsOffsDevDataAdsState,
            8
        );

Console.WriteLine(
    $"Read symbol info result: {infoResult}"
);

Console.WriteLine(
    $"Result type: {infoResult.GetType().FullName}"
);

foreach (var property in infoResult.GetType().GetProperties())
{
    try
    {
        Console.WriteLine(
            $"{property.Name} = {property.GetValue(infoResult)}"
        );
    }
    catch
    {
        // Ignore properties that cannot be read.
    }
}
        /*if (!infoResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to read symbol upload info: {infoResult}"
            );
        }*/

        /*infoResult.Data.CopyTo(infoBuffer, 0);

        if (infoResult != 8)
        {
            throw new InvalidOperationException(
                $"Expected 8 bytes from symbol upload info, " +
                $"received {infoResult}"
            );
        }*/
        
        var infoBuffer = infoResult.Data;

        var symbolCount = BinaryPrimitives.ReadUInt32LittleEndian(
            infoBuffer.Span.Slice(0, 4)
        );

        var symbolListLength = BinaryPrimitives.ReadUInt32LittleEndian(
            infoBuffer.Span.Slice(4, 4)
        );

        Console.WriteLine(
            $"PLC reports {symbolCount} symbols, " +
            $"symbol table size {symbolListLength} bytes."
        );

        //var symbolBuffer = new byte[symbolListLength];

        var symbolResult = _client.ReadAsResult(
            AdsGrpSymUpload,
            AdsOffsDevDataAdsState,
            checked((int)symbolListLength)
        );

        if (!symbolResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to read symbol table: {symbolResult}"
            );
        }

        /*symbolResult.Data.CopyTo(symbolBuffer, 0);

        if (symbolResult != symbolBuffer.Length)
        {
            throw new InvalidOperationException(
                $"Expected {symbolBuffer.Length} bytes from " +
                $"symbol upload, received {symbolResult}"
            );
        }*/
        
        var symbolBuffer = symbolResult.Data;

        return ParseSymbols(
            symbolBuffer,
            symbolCount
        );
    }

public void TestSymbolLoader()
{
    Console.WriteLine("Creating symbol loader...");

    var loader =
        (IAdsSymbolLoader)SymbolLoaderFactory.Create(
            _client,
            SymbolLoaderSettings.DefaultDynamic
        );

    Console.WriteLine("Symbol loader created.");

    var result = loader.GetSymbolsAsync(
        CancellationToken.None
    ).GetAwaiter().GetResult();

    Console.WriteLine(
        $"GetSymbolsAsync result: {result}"
    );

    Console.WriteLine(
        $"Result type: {result.GetType().FullName}"
    );

Console.WriteLine(
    $"Succeeded: {result.Succeeded}"
);

Console.WriteLine(
    $"ErrorCode: {result.ErrorCode}"
);

Console.WriteLine(
    $"Symbol collection type: {result.Symbols.GetType().FullName}"
);

Console.WriteLine();

var symbols = result.Symbols;

Console.WriteLine(
    $"Symbol count: {symbols.Count}"
);

foreach (var symbol in symbols)
{
    Console.WriteLine(
        $"Symbol type: {symbol.GetType().FullName}"
    );

    if (symbol.InstanceName == "MAIN")
    {
        Console.WriteLine();
        Console.WriteLine("MAIN members:");

        foreach (var member in symbol.SubSymbols)
        {
            Console.WriteLine(
                $"{member.GetType().Name}\t" +
                $"{member.InstancePath}\t" +
                $"{member.TypeName}"
            );
        }
    }
    
/*    foreach (var property in symbol.GetType().GetProperties())
    {
        try
        {
            Console.WriteLine(
                $"  {property.Name} = {property.GetValue(symbol)}"
            );
        }
        catch
        {
            // Ignore properties that cannot be read.
        }
    }*/

    //break; // Inspect just the first symbol for now.
}

}

    private static List<BeckhoffSymbol> ParseSymbols(
        ReadOnlyMemory<byte> buffer,
        uint symbolCount)
    {
        var symbols = new List<BeckhoffSymbol>();

        var position = 0;

        for (var i = 0; i < symbolCount; i++)
        {
            if (position + 30 > buffer.Length)
            {
                throw new InvalidOperationException(
                    "Unexpected end of symbol table."
                );
            }

            var readLength =
                BinaryPrimitives.ReadUInt32LittleEndian(
                    buffer.Span.Slice(position, 4)
                );

            var indexGroup =
                BinaryPrimitives.ReadUInt32LittleEndian(
                    buffer.Span.Slice(position + 4, 4)
                );

            var indexOffset =
                BinaryPrimitives.ReadUInt32LittleEndian(
                    buffer.Span.Slice(position + 8, 4)
                );

            var nameLength =
                BinaryPrimitives.ReadUInt16LittleEndian(
                    buffer.Span.Slice(position + 24, 2)
                );

            var typeLength =
                BinaryPrimitives.ReadUInt16LittleEndian(
                    buffer.Span.Slice(position + 26, 2)
                );

            var commentLength =
                BinaryPrimitives.ReadUInt16LittleEndian(
                    buffer.Span.Slice(position + 28, 2)
                );

            var nameStart = position + 30;
            var typeStart = nameStart + nameLength + 1;
            var commentStart = typeStart + typeLength + 1;

            var name = Encoding.UTF8.GetString(
                buffer.Span.Slice(nameStart, nameLength)
                /*buffer,
                nameStart,
                nameLength*/
            );

            var type = Encoding.UTF8.GetString(
                buffer.Span.Slice(typeStart, typeLength)
                /*buffer,
                typeStart,
                typeLength*/
            );

            var comment = Encoding.UTF8.GetString(
                buffer.Span.Slice(commentStart, commentLength)
                /*buffer,
                commentStart,
                commentLength*/
            );

            if (!name.StartsWith("."))
            {
                symbols.Add(
                    new BeckhoffSymbol(
                        name,
                        type,
                        comment,
                        indexGroup,
                        indexOffset
                    )
                );
            }

            position += checked((int)readLength);
        }

        return symbols;
    }
}