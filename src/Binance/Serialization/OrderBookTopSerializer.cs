using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class OrderBookTopSerializer : IOrderBookTopSerializer
    {
        private const string KeySymbol = "symbol";
        private const string KeyBidPrice = "bidPrice";
        private const string KeyBidQuantity = "bidQty";
        private const string KeyAskPrice = "askPrice";
        private const string KeyAskQuantity = "askQty";

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
                new JProperty(KeySymbol, orderBookTop.Symbol),
                new JProperty(KeyBidPrice, orderBookTop.Bid.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyBidQuantity, orderBookTop.Bid.Quantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyAskPrice, orderBookTop.Ask.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyAskQuantity, orderBookTop.Ask.Quantity.ToString(CultureInfo.InvariantCulture))
            };

            return jObject.ToString(Formatting.None);
        }

        private static OrderBookTop DeserializeOrderBookTop(JToken jToken)
        {
            return OrderBookTop.Create(
                jToken[KeySymbol].Value<string>(),
                jToken[KeyBidPrice].Value<decimal>(),
                jToken[KeyBidQuantity].Value<decimal>(),
                jToken[KeyAskPrice].Value<decimal>(),
                jToken[KeyAskQuantity].Value<decimal>());
        }
    }
}
