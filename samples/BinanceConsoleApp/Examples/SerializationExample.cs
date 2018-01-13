using System;
using System.Threading.Tasks;
using Binance;
using Binance.Api;
using Binance.Market;
using Binance.Serialization;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace BinanceConsoleApp
{
    internal class SerializationExample
    {
        public static async Task ExampleMain(string[] args)
        {
            var api = new BinanceApi();

            var orderBook = await api.GetOrderBookAsync(Symbol.BTC_USDT, 50);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new OrderBookJsonConverter());

            // Serialize order book.
            var json = JsonConvert.SerializeObject(orderBook, settings);

            // Deserialize order book.
            var orderBookDeserialized = JsonConvert.DeserializeObject<OrderBook>(json, settings);

            Console.WriteLine("...press any key to continue.");
            Console.ReadKey(true);
        }
    }
}
