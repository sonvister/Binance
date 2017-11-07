using Binance;
using Binance.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BinanceCodeGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load configuration.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, false)
                .Build();

            // Read list of known quote currencies.
            var quoteCurrencies = configuration
                .GetSection("QuoteCurrencies").GetChildren()
                .Select(c => c.Value).ToArray();
            
            // Initailize base currency lists.
            var currencyPairs = new Dictionary<string, List<string>>();
            foreach (var currency in quoteCurrencies)
            {
                currencyPairs[currency] = new List<string>();
            }

            // Initialize assets list.
            var assets = new List<string>(quoteCurrencies);

            long timestamp;

            using (var api = new BinanceApi())
            {
                // Get the current timestamp.
                timestamp = await api.GetTimestampAsync();

                // Get latest currency pairs (symbols).
                var symbols = await api.SymbolsAsync();

                // Organize base currencies into quote currency lists.
                foreach (var symbol in symbols)
                {
                    var match = false;

                    foreach (var currency in quoteCurrencies)
                    {
                        if (symbol.EndsWith(currency))
                        {
                            var asset = symbol.Substring(0, symbol.IndexOf(currency));

                            if (!assets.Contains(asset))
                                assets.Add(asset);

                            currencyPairs[currency].Add(asset);

                            match = true;
                        }
                    }

                    // If no matching quote currency is found.
                    if (!match)
                    {
                        Console.WriteLine($"!! Symbol does not match known quote currencies: {symbol}");
                    }
                }
            }

            // Read the symbol template file.
            var lines = (await File.ReadAllLinesAsync("Symbol.template.cs")).ToList();

            // Replace timestamp.
            var index = lines.FindIndex(l => l.Contains("<<insert timestamp>>"));
            lines[index] = $"        public static readonly long LastUpdateAt = {timestamp};";

            index = lines.FindIndex(l => l.Contains($"<<insert symbols>>"));
            lines.RemoveAt(index);

            // Insert definition for each currency pair.
            foreach (var quoteCurrency in quoteCurrencies)
            {
                lines.Insert(index, $"        // {quoteCurrency}");
                index++;

                // Sort base currencies.
                currencyPairs[quoteCurrency].Sort();

                foreach (var baseCurrency in currencyPairs[quoteCurrency])
                {
                    lines.Insert(index, $"        public static readonly string {baseCurrency}_{quoteCurrency} = \"{baseCurrency}{quoteCurrency}\";");
                    index++;
                }

                if (!quoteCurrency.Equals(quoteCurrencies.Last()))
                {
                    lines.Insert(index, string.Empty);
                    index++;
                }
            }

            // Save the generated source code (replacing original).
            await File.WriteAllLinesAsync("../../src/Binance/Symbol.cs", lines);

            // Read the asset template file.
            lines = (await File.ReadAllLinesAsync("Asset.template.cs")).ToList();

            // Replace timestamp.
            index = lines.FindIndex(l => l.Contains("<<insert timestamp>>"));
            lines[index] = $"        public static readonly long LastUpdateAt = {timestamp};";

            index = lines.FindIndex(l => l.Contains($"<<insert assets>>"));
            lines.RemoveAt(index);

            // Sort assets.
            assets.Sort();

            // Insert definition for each asset.
            foreach (var asset in assets)
            {
                lines.Insert(index, $"        public static readonly string {asset} = \"{asset}\";");
                index++;
            }

            // Save the generated source code (replacing original).
            await File.WriteAllLinesAsync("../../src/Binance/Asset.cs", lines);

            Console.WriteLine();
            Console.WriteLine("  ...press any key to close window.");
            Console.ReadKey(true);
        }
    }
}
