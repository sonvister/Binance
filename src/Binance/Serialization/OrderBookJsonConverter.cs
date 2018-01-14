using System;
using System.Linq;
using Binance.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public sealed class OrderBookJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(OrderBook));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            var symbol = jObject[nameof(OrderBook.Symbol)].Value<string>();

            var lastUpdateId = jObject[nameof(OrderBook.LastUpdateId)].Value<long>();

            var bids = jObject[nameof(OrderBook.Bids)]
                .Select(_ => (_[0].Value<decimal>(), _[1].Value<decimal>()))
                .ToArray();

            var asks = jObject[nameof(OrderBook.Asks)]
                .Select(_ => (_[0].Value<decimal>(), _[1].Value<decimal>()))
                .ToArray();

            return new OrderBook(symbol, lastUpdateId, bids, asks);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var orderBook = value as OrderBook;

            if (orderBook == null)
                return;

            var jObject = new JObject
            {
                new JProperty(nameof(OrderBook.Symbol), orderBook.Symbol),

                new JProperty(nameof(OrderBook.LastUpdateId), orderBook.LastUpdateId),

                new JProperty(nameof(OrderBook.Bids), orderBook.Bids.Select(_ => new JArray { _.Price, _.Quantity })),

                new JProperty(nameof(OrderBook.Asks), orderBook.Asks.Select(_ => new JArray { _.Price, _.Quantity }))
            };

            jObject.WriteTo(writer);
        }
    }
}
