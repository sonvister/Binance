using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class SymbolAveragePriceSerializer : ISymbolAveragePriceSerializer
    {
        private const string KeyMinutes = "mins";
        private const string KeyAvgPrice = "price";

        public virtual SymbolAveragePrice Deserialize(string symbol, string json)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return DeserializeSymbolPrice(symbol, JObject.Parse(json));
        }

        public virtual string Serialize(SymbolAveragePrice symbolAveragePrice)
        {
            Throw.IfNull(symbolAveragePrice, nameof(symbolAveragePrice));

            var jObject = new JObject
            {
                new JProperty(KeyMinutes, symbolAveragePrice.Minutes),
                new JProperty(KeyAvgPrice, symbolAveragePrice.Value)
            };

            return jObject.ToString(Formatting.None);
        }

        private static SymbolAveragePrice DeserializeSymbolPrice(string symbol, JToken jToken)
        {
            return new SymbolAveragePrice(symbol,
                jToken[KeyMinutes].Value<int>(),
                jToken[KeyAvgPrice].Value<decimal>());
        }
    }
}
