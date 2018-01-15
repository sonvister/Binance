using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binance.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class AggregateTradeSerializer : IAggregateTradeSerializer
    {
        private const string Key_Symbol = "symbol";
        private const string Key_Id = "id";
        private const string Key_Price = "price";
        private const string Key_Quantity = "qty";
        private const string Key_FirstTradeId = "firstTradeId";
        private const string Key_LastTradeId = "lastTradeId";
        private const string Key_Time = "time";
        private const string Key_IsBuyerMaker = "isBuyerMaker";
        private const string Key_IsBestPriceMatch = "isBestMatch";

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
                new JProperty(Key_Symbol, trade.Symbol),
                new JProperty(Key_Id, trade.Id),
                new JProperty(Key_Price, trade.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Quantity, trade.Quantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_FirstTradeId, trade.FirstTradeId),
                new JProperty(Key_LastTradeId, trade.LastTradeId),
                new JProperty(Key_Time, trade.Timestamp),
                new JProperty(Key_IsBuyerMaker, trade.IsBuyerMaker),
                new JProperty(Key_IsBestPriceMatch, trade.IsBestPriceMatch)
            };

            return jObject.ToString(Formatting.None);
        }

        private static AggregateTrade DeserializeTrade(JToken jToken, string symbol = null)
        {
            if (symbol == null)
            {
                return new AggregateTrade(
                    jToken[Key_Symbol].Value<string>(),
                    jToken[Key_Id].Value<long>(),
                    jToken[Key_Price].Value<decimal>(),
                    jToken[Key_Quantity].Value<decimal>(),
                    jToken[Key_FirstTradeId].Value<long>(),
                    jToken[Key_LastTradeId].Value<long>(),
                    jToken[Key_Time].Value<long>(),
                    jToken[Key_IsBuyerMaker].Value<bool>(),
                    jToken[Key_IsBestPriceMatch].Value<bool>());
            }
            else
            {
                return new AggregateTrade(
                    symbol,
                    jToken["a"].Value<long>(), // ID
                    jToken["p"].Value<decimal>(), // price
                    jToken["q"].Value<decimal>(), // quantity
                    jToken["f"].Value<long>(), // first trade ID
                    jToken["l"].Value<long>(), // last trade ID
                    jToken["T"].Value<long>(), // timestamp
                    jToken["m"].Value<bool>(), // is buyer maker
                    jToken["M"].Value<bool>()); // is best price match
            }
        }
    }
}
