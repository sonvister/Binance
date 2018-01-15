using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binance.Account;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class AccountTradeSerializer : IAccountTradeSerializer
    {
        private const string Key_Symbol = "symbol";
        private const string Key_Id = "id";
        private const string Key_OrderId = "orderId";
        private const string Key_Price = "price";
        private const string Key_Quantity = "qty";
        private const string Key_Commission = "commission";
        private const string Key_CommissionAsset = "commissionAsset";
        private const string Key_Time = "time";
        private const string Key_IsBuyer = "isBuyer";
        private const string Key_IsMaker = "isMaker";
        private const string Key_IsBestPriceMatch = "isBestMatch";

        public virtual AccountTrade Deserialize(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return DeserializeTrade(JObject.Parse(json));
        }

        public virtual IEnumerable<AccountTrade> DeserializeMany(string json, string symbol)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            return JArray.Parse(json)
                .Select(jToken => DeserializeTrade(jToken, symbol))
                .ToArray();
        }

        public virtual string Serialize(AccountTrade trade)
        {
            Throw.IfNull(trade, nameof(trade));

            var jObject = new JObject
            {
                new JProperty(Key_Symbol, trade.Symbol),
                new JProperty(Key_Id, trade.Id),
                new JProperty(Key_OrderId, trade.OrderId),
                new JProperty(Key_Price, trade.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Quantity, trade.Quantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Commission, trade.Commission.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_CommissionAsset, trade.CommissionAsset),
                new JProperty(Key_Time, trade.Timestamp),
                new JProperty(Key_IsBuyer, trade.IsBuyer),
                new JProperty(Key_IsMaker, trade.IsMaker),
                new JProperty(Key_IsBestPriceMatch, trade.IsBestPriceMatch)
            };

            return jObject.ToString(Formatting.None);
        }

        private static AccountTrade DeserializeTrade(JToken jToken, string symbol = null)
        {
            if (symbol == null)
            {
                return new AccountTrade(
                    jToken[Key_Symbol].Value<string>(),
                    jToken[Key_Id].Value<long>(),
                    jToken[Key_OrderId].Value<long>(),
                    jToken[Key_Price].Value<decimal>(),
                    jToken[Key_Quantity].Value<decimal>(),
                    jToken[Key_Commission].Value<decimal>(),
                    jToken[Key_CommissionAsset].Value<string>(),
                    jToken[Key_Time].Value<long>(),
                    jToken[Key_IsBuyer].Value<bool>(),
                    jToken[Key_IsMaker].Value<bool>(),
                    jToken[Key_IsBestPriceMatch].Value<bool>());
            }
            else
            {
                return new AccountTrade(
                    symbol,
                    jToken["id"].Value<long>(),
                    jToken["orderId"].Value<long>(),
                    jToken["price"].Value<decimal>(),
                    jToken["qty"].Value<decimal>(),
                    jToken["commission"].Value<decimal>(),
                    jToken["commissionAsset"].Value<string>(),
                    jToken["time"].Value<long>(),
                    jToken["isBuyer"].Value<bool>(),
                    jToken["isMaker"].Value<bool>(),
                    jToken["isBestMatch"].Value<bool>());
            }
        }
    }
}
