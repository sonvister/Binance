using System.Collections.Generic;
using System.Linq;
using Binance.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class SymbolPriceSerializer : ISymbolPriceSerializer
    {
        private const string Key_Symbol = "symbol";
        private const string Key_Price = "price";

        public virtual SymbolPrice Deserialize(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return DeserializeSymbolPrice(JObject.Parse(json));
        }

        public virtual IEnumerable<SymbolPrice> DeserializeMany(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return JArray.Parse(json)
                .Select(DeserializeSymbolPrice)
                .Where(_ => _.Symbol != "123456" && _.Symbol != "ETC") // HACK
                .ToArray();
        }

        public virtual string Serialize(SymbolPrice symbolPrice)
        {
            Throw.IfNull(symbolPrice, nameof(symbolPrice));

            var jObject = new JObject
            {
                new JProperty(Key_Symbol, symbolPrice.Symbol),
                new JProperty(Key_Price, symbolPrice.Value)
            };

            return jObject.ToString(Formatting.None);
        }

        private static SymbolPrice DeserializeSymbolPrice(JToken jToken)
        {
            return new SymbolPrice(
                jToken["symbol"].Value<string>(),
                jToken["price"].Value<decimal>());
        }
    }
}
