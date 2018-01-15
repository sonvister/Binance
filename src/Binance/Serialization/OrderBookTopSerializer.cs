using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binance.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class OrderBookTopSerializer : IOrderBookTopSerializer
    {
        private const string Key_Symbol = "symbol";
        private const string Key_BidPrice = "bidPrice";
        private const string Key_BidQuantity = "bidQty";
        private const string Key_AskPrice = "askPrice";
        private const string Key_AskQuantity = "askQty";

        public virtual OrderBookTop Deserialize(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return DeserializeOrderBookTop(JObject.Parse(json));
        }

        public virtual IEnumerable<OrderBookTop> DeserializeMany(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return JArray.Parse(json)
                .Select(DeserializeOrderBookTop)
                .ToArray();
        }

        public virtual string Serialize(OrderBookTop orderBookTop)
        {
            Throw.IfNull(orderBookTop, nameof(orderBookTop));

            var jObject = new JObject
            {
                new JProperty(Key_Symbol, orderBookTop.Symbol),
                new JProperty(Key_BidPrice, orderBookTop.Bid.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_BidQuantity, orderBookTop.Bid.Quantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_AskPrice, orderBookTop.Ask.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_AskQuantity, orderBookTop.Ask.Quantity.ToString(CultureInfo.InvariantCulture))
            };

            return jObject.ToString(Formatting.None);
        }

        private static OrderBookTop DeserializeOrderBookTop(JToken jToken)
        {
            return OrderBookTop.Create(
                jToken[Key_Symbol].Value<string>(),
                jToken[Key_BidPrice].Value<decimal>(),
                jToken[Key_BidQuantity].Value<decimal>(),
                jToken[Key_AskPrice].Value<decimal>(),
                jToken[Key_AskQuantity].Value<decimal>());
        }
    }
}
