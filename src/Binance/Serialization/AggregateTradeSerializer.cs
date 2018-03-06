using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class AggregateTradeSerializer : IAggregateTradeSerializer
    {
        private const string KeySymbol = "symbol";
        private const string KeyId = "id";
        private const string KeyPrice = "price";
        private const string KeyQuantity = "qty";
        private const string KeyFirstTradeId = "firstTradeId";
        private const string KeyLastTradeId = "lastTradeId";
        private const string KeyTime = "time";
        private const string KeyIsBuyerMaker = "isBuyerMaker";
        private const string KeyIsBestPriceMatch = "isBestMatch";

        public virtual AggregateTrade Deserialize(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return DeserializeTrade(JObject.Parse(json));
        }

        public virtual IEnumerable<AggregateTrade> DeserializeMany(string json, string symbol)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            return JArray.Parse(json)
                .Select(item => DeserializeTrade(item, symbol))
                .ToArray();
        }

        public virtual string Serialize(AggregateTrade trade)
        {
            Throw.IfNull(trade, nameof(trade));

            var jObject = new JObject
            {
                new JProperty(KeySymbol, trade.Symbol),
                new JProperty(KeyId, trade.Id),
                new JProperty(KeyPrice, trade.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyQuantity, trade.Quantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyFirstTradeId, trade.FirstTradeId),
                new JProperty(KeyLastTradeId, trade.LastTradeId),
                new JProperty(KeyTime, trade.Time.ToTimestamp()),
                new JProperty(KeyIsBuyerMaker, trade.IsBuyerMaker),
                new JProperty(KeyIsBestPriceMatch, trade.IsBestPriceMatch)
            };

            return jObject.ToString(Formatting.None);
        }

        private static AggregateTrade DeserializeTrade(JToken jToken, string symbol = null)
        {
            if (symbol == null)
            {
                return new AggregateTrade(
                    jToken[KeySymbol].Value<string>(),
                    jToken[KeyId].Value<long>(),
                    jToken[KeyPrice].Value<decimal>(),
                    jToken[KeyQuantity].Value<decimal>(),
                    jToken[KeyFirstTradeId].Value<long>(),
                    jToken[KeyLastTradeId].Value<long>(),
                    jToken[KeyTime].Value<long>().ToDateTime(),
                    jToken[KeyIsBuyerMaker].Value<bool>(),
                    jToken[KeyIsBestPriceMatch].Value<bool>());
            }

            return new AggregateTrade(
                symbol,
                jToken["a"].Value<long>(),    // ID
                jToken["p"].Value<decimal>(), // price
                jToken["q"].Value<decimal>(), // quantity
                jToken["f"].Value<long>(),    // first trade ID
                jToken["l"].Value<long>(),    // last trade ID
                jToken["T"].Value<long>()
                    .ToDateTime(),            // time
                jToken["m"].Value<bool>(),    // is buyer maker
                jToken["M"].Value<bool>());   // is best price match
        }
    }
}
