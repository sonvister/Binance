using System;
using System.Linq;
using Binance.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public sealed class OrderBookJsonConverter : JsonConverter
    {
        /// <summary>
        /// Get or set flag to include OrderBook.Symbol in JSON.
        /// </summary>
        public bool SerializeSymbol { get; set; } = true;

        private const string Key_Symbol = "symbol";
        private const string Key_LastUpdateId = "lastUpdateId";
        private const string Key_Bids = "bids";
        private const string Key_Asks = "asks";

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(OrderBook));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            var symbol = jObject[Key_Symbol].Value<string>();

            var lastUpdateId = jObject[Key_LastUpdateId].Value<long>();

            var bids = jObject[Key_Bids]
                .Select(_ => (_[0].Value<decimal>(), _[1].Value<decimal>()))
                .ToArray();

            var asks = jObject[Key_Asks]
                .Select(_ => (_[0].Value<decimal>(), _[1].Value<decimal>()))
                .ToArray();

            return new OrderBook(symbol, lastUpdateId, bids, asks);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var orderBook = value as OrderBook;

            if (orderBook == null)
                return;

            var jObject = new JObject();

            if (SerializeSymbol)
            {
                jObject.Add(new JProperty(Key_Symbol, orderBook.Symbol));
            }

            jObject.Add(new JProperty(Key_LastUpdateId, orderBook.LastUpdateId));

            jObject.Add(new JProperty(Key_Bids, orderBook.Bids.Select(_ => new JArray { _.Price, _.Quantity })));

            jObject.Add(new JProperty(Key_Asks, orderBook.Asks.Select(_ => new JArray { _.Price, _.Quantity })));

            jObject.WriteTo(writer);
        }

        /// <summary>
        /// Insert symbol into JSON.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string InsertSymbol(string json, string symbol)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            return json.Insert(1, $"\"symbol\":\"{symbol.FormatSymbol()}\",");
        }
    }
}
