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
        private const string Key_Symbol = "symbol";
        private const string Key_Id = "id";
        private const string Key_Price = "price";
        private const string Key_Quantity = "qty";
        private const string Key_Time = "time";
        private const string Key_IsBuyerMaker = "isBuyerMaker";
        private const string Key_IsBestPriceMatch = "isBestMatch";
        private const string Key_BuyerOrderId = "buyerOrderId";
        private const string Key_SellerOrderId = "sellerOrderId";

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
                new JProperty(Key_Symbol, trade.Symbol),
                new JProperty(Key_Id, trade.Id),
                new JProperty(Key_Price, trade.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Quantity, trade.Quantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Time, trade.Timestamp),
                new JProperty(Key_IsBuyerMaker, trade.IsBuyerMaker),
                new JProperty(Key_IsBestPriceMatch, trade.IsBestPriceMatch)
            };

            if (trade.BuyerOrderId != BinanceApi.NullId)
            {
                jObject.Add(new JProperty(Key_BuyerOrderId, trade.BuyerOrderId));
            }

            if (trade.SellerOrderId != BinanceApi.NullId)
            {
                jObject.Add(new JProperty(Key_SellerOrderId, trade.SellerOrderId));
            }

            return jObject.ToString(Formatting.None);
        }

        private static Trade DeserializeTrade(JToken jToken, string symbol = null)
        {
            return new Trade(
                jToken[Key_Symbol]?.Value<string>() ?? symbol,
                jToken[Key_Id].Value<long>(), // ID
                jToken[Key_Price].Value<decimal>(), // price
                jToken[Key_Quantity].Value<decimal>(), // quantity
                jToken[Key_BuyerOrderId]?.Value<long>() ?? BinanceApi.NullId, // buyer order ID
                jToken[Key_SellerOrderId]?.Value<long>() ?? BinanceApi.NullId, // seller order ID
                jToken[Key_Time].Value<long>(), // timestamp
                jToken[Key_IsBuyerMaker].Value<bool>(), // is buyer maker
                jToken[Key_IsBestPriceMatch].Value<bool>()); // is best price match
        }
    }
}
