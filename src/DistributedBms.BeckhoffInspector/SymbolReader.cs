/*using System.Buffers.Binary;
using System.Text;*/
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace DistributedBms.BeckhoffInspector;


public class SymbolReader
{
    /*private const uint AdsGrpSymUploadInfo2 = 0xF00F;
    private const uint AdsGrpSymUpload = 0xF00B;
    private const uint AdsOffsDevDataAdsState = 0x0000;*/

    private readonly AdsClient _client;

    public SymbolReader(AdsClient client)
    {
        _client = client;
    }

    public List<BeckhoffSymbol> GetAllSymbols()
    {
        var loader =
            (IAdsSymbolLoader)SymbolLoaderFactory.Create(
                _client,
                SymbolLoaderSettings.DefaultDynamic
            );

        var result = loader.GetSymbolsAsync(
            CancellationToken.None
        ).GetAwaiter().GetResult();

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to read symbols: {result.ErrorCode}"
            );
        }

        var symbols = new List<BeckhoffSymbol>();

        foreach (var symbol in result.Symbols)
        {
            CollectSymbols(symbol, symbols);
        }

        return symbols;
    }


    private static void CollectSymbols(
        ISymbol symbol,
        List<BeckhoffSymbol> symbols)
    {
        foreach (var child in symbol.SubSymbols)
        {   
            if (child.IsContainerType)
            {
                CollectSymbols(child, symbols);
                continue;
            }

            var innerProperty =
                child.GetType().GetProperty("_InnerSymbol");

            if (innerProperty == null)
            {
                continue;
            }

            var inner = innerProperty.GetValue(child);

            if (inner == null)
            {
                continue;
            }

            var innerType = inner.GetType();

            var indexGroup =
                (uint)innerType
                    .GetProperty("IndexGroup")!
                    .GetValue(inner)!;

            var indexOffset =
                (uint)innerType
                    .GetProperty("IndexOffset")!
                    .GetValue(inner)!;

            symbols.Add(
                new BeckhoffSymbol(
                    child.InstancePath,
                    child.TypeName,
                    child.Comment ?? "",
                    indexGroup,
                    indexOffset
                )
            );
        }
    }

}