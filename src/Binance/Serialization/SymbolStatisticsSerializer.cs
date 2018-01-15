using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binance.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class SymbolStatisticsSerializer : ISymbolStatisticsSerializer
    {
        private const string Key_Symbol = "symbol";
        private const string Key_PriceChange = "priceChange";
        private const string Key_PriceChangePercent = "priceChangePercent";
        private const string Key_WeightedAveragePrice = "weightedAvgPrice";
        private const string Key_PreviousClosePrice = "prevClosePrice";
        private const string Key_LastPrice = "lastPrice";
        private const string Key_LastQuantity = "lastQty";
        private const string Key_BidPrice = "bidPrice";
        private const string Key_BidQuantity = "bidQty";
        private const string Key_AskPrice = "askPrice";
        private const string Key_AskQuantity = "askQty";
        private const string Key_OpenPrice = "openPrice";
        private const string Key_HighPrice = "highPrice";
        private const string Key_LowPrice = "lowPrice";
        private const string Key_Volume = "volume";
        private const string Key_QuoteVolume = "quoteVolume";
        private const string Key_OpenTime = "openTime";
        private const string Key_CloseTime = "closeTime";
        private const string Key_FirstTradeId = "firstId";
        private const string Key_LastTradeId = "lastId";
        private const string Key_TradeCount = "count";

        public virtual SymbolStatistics Deserialize(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return DeserializeSymbolStatistics(JObject.Parse(json));
        }

        public virtual IEnumerable<SymbolStatistics> DeserializeMany(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            return JArray.Parse(json)
                .Select(DeserializeSymbolStatistics)
                .ToArray();
        }

        public virtual string Serialize(SymbolStatistics statistics)
        {
            Throw.IfNull(statistics, nameof(statistics));

            var jObject = new JObject
            {
                new JProperty(Key_Symbol, statistics.Symbol),
                new JProperty(Key_PriceChange, statistics.PriceChange.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_PriceChangePercent, statistics.PriceChangePercent.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_WeightedAveragePrice, statistics.WeightedAveragePrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_PreviousClosePrice, statistics.PreviousClosePrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_LastPrice, statistics.LastPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_LastQuantity, statistics.LastQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_BidPrice, statistics.BidPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_BidQuantity, statistics.BidQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_AskPrice, statistics.AskPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_AskQuantity, statistics.AskQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_OpenPrice, statistics.OpenPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_HighPrice, statistics.HighPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_LowPrice, statistics.LowPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Volume, statistics.Volume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_QuoteVolume, statistics.QuoteVolume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_OpenTime, statistics.OpenTime),
                new JProperty(Key_CloseTime, statistics.CloseTime),
                new JProperty(Key_FirstTradeId, statistics.FirstTradeId),
                new JProperty(Key_LastTradeId, statistics.LastTradeId),
                new JProperty(Key_TradeCount, statistics.TradeCount),
            };

            return jObject.ToString(Formatting.None);
        }

        private static SymbolStatistics DeserializeSymbolStatistics(JToken jToken)
        {
            return new SymbolStatistics(
                jToken[Key_Symbol].Value<string>(),
                TimeSpan.FromHours(24),
                jToken[Key_PriceChange].Value<decimal>(),
                jToken[Key_PriceChangePercent].Value<decimal>(),
                jToken[Key_WeightedAveragePrice].Value<decimal>(),
                jToken[Key_PreviousClosePrice].Value<decimal>(),
                jToken[Key_LastPrice].Value<decimal>(),
                jToken[Key_LastQuantity].Value<decimal>(),
                jToken[Key_BidPrice].Value<decimal>(),
                jToken[Key_BidQuantity].Value<decimal>(),
                jToken[Key_AskPrice].Value<decimal>(),
                jToken[Key_AskQuantity].Value<decimal>(),
                jToken[Key_OpenPrice].Value<decimal>(),
                jToken[Key_HighPrice].Value<decimal>(),
                jToken[Key_LowPrice].Value<decimal>(),
                jToken[Key_Volume].Value<decimal>(),
                jToken[Key_QuoteVolume].Value<decimal>(),
                jToken[Key_OpenTime].Value<long>(),
                jToken[Key_CloseTime].Value<long>(),
                jToken[Key_FirstTradeId].Value<long>(),
                jToken[Key_LastTradeId].Value<long>(),
                jToken[Key_TradeCount].Value<long>());
        }
    }
}
