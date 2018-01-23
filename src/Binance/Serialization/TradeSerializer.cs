using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binance.Api;
using Binance.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class TradeSerializer : ITradeSerializer
    {
        private const string KeySymbol = "symbol";
        private const string KeyId = "id";
        private const string KeyPrice = "price";
        private const string KeyQuantity = "qty";
        private const string KeyTime = "time";
        private const string KeyIsBuyerMaker = "isBuyerMaker";
        private const string KeyIsBestPriceMatch = "isBestMatch";
        private const string KeyBuyerOrderId = "buyerOrderId";
        private const string KeySellerOrderId = "sellerOrderId";

        public virtual Trade Deserialize(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return DeserializeTrade(JObject.Parse(json));
        }

        public virtual IEnumerable<Trade> DeserializeMany(string json, string symbol)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            return JArray.Parse(json)
                .Select(item => DeserializeTrade(item, symbol))
                .ToArray();
        }

        public virtual string Serialize(Trade trade)
        {
            Throw.IfNull(trade, nameof(trade));

            var jObject = new JObject
            {
                new JProperty(KeySymbol, trade.Symbol),
                new JProperty(KeyId, trade.Id),
                new JProperty(KeyPrice, trade.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyQuantity, trade.Quantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyTime, trade.Timestamp),
                new JProperty(KeyIsBuyerMaker, trade.IsBuyerMaker),
                new JProperty(KeyIsBestPriceMatch, trade.IsBestPriceMatch)
            };

            if (trade.BuyerOrderId != BinanceApi.NullId)
            {
                jObject.Add(new JProperty(KeyBuyerOrderId, trade.BuyerOrderId));
            }

            if (trade.SellerOrderId != BinanceApi.NullId)
            {
                jObject.Add(new JProperty(KeySellerOrderId, trade.SellerOrderId));
            }

            return jObject.ToString(Formatting.None);
        }

        private static Trade DeserializeTrade(JToken jToken, string symbol = null)
        {
            return new Trade(
                jToken[KeySymbol]?.Value<string>() ?? symbol,
                jToken[KeyId].Value<long>(), // ID
                jToken[KeyPrice].Value<decimal>(), // price
                jToken[KeyQuantity].Value<decimal>(), // quantity
                jToken[KeyBuyerOrderId]?.Value<long>() ?? BinanceApi.NullId, // buyer order ID
                jToken[KeySellerOrderId]?.Value<long>() ?? BinanceApi.NullId, // seller order ID
                jToken[KeyTime].Value<long>(), // timestamp
                jToken[KeyIsBuyerMaker].Value<bool>(), // is buyer maker
                jToken[KeyIsBestPriceMatch].Value<bool>()); // is best price match
        }
    }
}
