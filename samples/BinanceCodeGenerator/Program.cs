using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Api;

namespace BinanceCodeGenerator
{
    internal class Program
    {
        private static async Task Main()
        {
            var assets = new List<Asset>();

            long timestamp;

            List<Symbol> symbols;

            var api = new BinanceApi();

            // Get the current timestamp.
            timestamp = await api.GetTimestampAsync();

            // Get latest currency pairs (symbols).
            symbols = (await api.GetSymbolsAsync()).ToList();

            // Get assets.
            foreach (var symbol in symbols)
            {
                if (!assets.Contains(symbol.BaseAsset))
                    assets.Add(symbol.BaseAsset);

                if (!assets.Contains(symbol.QuoteAsset))
                    assets.Add(symbol.QuoteAsset);
            }

            // Read the symbol template file.
            var lines = (await File.ReadAllLinesAsync("Symbol.template.cs")).ToList();

            // Replace timestamp.
            var index = lines.FindIndex(l => l.Contains("<<insert timestamp>>"));
            lines[index] = $"        public static readonly long LastUpdateAt = {timestamp};";

            index = lines.FindIndex(l => l.Contains("<<insert symbols>>"));
            lines.RemoveAt(index);

            // Sort symbols.
            symbols.Sort();

            var groups = symbols.GroupBy(s => s.QuoteAsset);

            // Insert definition for each currency pair (symbol).
            foreach (var group in groups)
            {
                lines.Insert(index++, $"        // {group.First().QuoteAsset}");

                foreach (var symbol in group)
                {
                    if ($"{symbol.BaseAsset}{symbol.QuoteAsset}" == symbol.ToString())
                    {
                        lines.Insert(index++, $"        public static readonly Symbol {symbol.BaseAsset}_{symbol.QuoteAsset} = new Symbol(Asset.{symbol.BaseAsset}, Asset.{symbol.QuoteAsset}, {symbol.BaseMinQuantity}m, {symbol.BaseMaxQuantity}m, {symbol.QuoteIncrement}m);");
                    }
                    else
                    {
                        lines.Insert(index++, $"        public static readonly Symbol {symbol} = new Symbol(Asset.{symbol.BaseAsset}, Asset.{symbol.QuoteAsset}, {symbol.BaseMinQuantity}m, {symbol.BaseMaxQuantity}m, {symbol.QuoteIncrement}m, {symbol});");
                    }
                }

                lines.Insert(index++, string.Empty);
            }
            lines.RemoveAt(index);

            index = lines.FindIndex(l => l.Contains("<<insert symbol definitions>>"));
            lines.RemoveAt(index);

            foreach(var symbol in symbols)
            {
                if ($"{symbol.BaseAsset}{symbol.QuoteAsset}" == symbol.ToString())
                {
                    lines.Insert(index++, $"            {{ \"{symbol}\", {symbol.BaseAsset}_{symbol.QuoteAsset} }},");
                }
                else
                {
                    lines.Insert(index++, $"            {{ \"{symbol}\", {symbol}}},");
                }
            }

            // Save the generated source code (replacing original).
            await File.WriteAllLinesAsync("../../src/Binance/Symbol.cs", lines);

            // Read the asset template file.
            lines = (await File.ReadAllLinesAsync("Asset.template.cs")).ToList();

            // Replace timestamp.
            index = lines.FindIndex(l => l.Contains("<<insert timestamp>>"));
            lines[index] = $"        public static readonly long LastUpdateAt = {timestamp};";

            index = lines.FindIndex(l => l.Contains("<<insert assets>>"));
            lines.RemoveAt(index);

            // Sort assets.
            assets.Sort();

            // Insert definition for each asset.
            foreach (var asset in assets)
            {
                lines.Insert(index++, $"        public static readonly Asset {asset} = new Asset(\"{asset}\", {asset.Precision});");
            }

            index = lines.FindIndex(l => l.Contains("<<insert asset definitions>>"));
            lines.RemoveAt(index);

            foreach (var asset in assets)
            {
                lines.Insert(index++, $"            {{ \"{asset}\", {asset} }},");
            }

            // Save the generated source code (replacing original).
            await File.WriteAllLinesAsync("../../src/Binance/Asset.cs", lines);

            Console.WriteLine();
            Console.WriteLine("  ...press any key to close window.");
            Console.ReadKey(true);
        }
    }
}
