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
        private const string KeySymbol = "symbol";
        private const string KeyPriceChange = "priceChange";
        private const string KeyPriceChangePercent = "priceChangePercent";
        private const string KeyWeightedAveragePrice = "weightedAvgPrice";
        private const string KeyPreviousClosePrice = "prevClosePrice";
        private const string KeyLastPrice = "lastPrice";
        private const string KeyLastQuantity = "lastQty";
        private const string KeyBidPrice = "bidPrice";
        private const string KeyBidQuantity = "bidQty";
        private const string KeyAskPrice = "askPrice";
        private const string KeyAskQuantity = "askQty";
        private const string KeyOpenPrice = "openPrice";
        private const string KeyHighPrice = "highPrice";
        private const string KeyLowPrice = "lowPrice";
        private const string KeyVolume = "volume";
        private const string KeyQuoteVolume = "quoteVolume";
        private const string KeyOpenTime = "openTime";
        private const string KeyCloseTime = "closeTime";
        private const string KeyFirstTradeId = "firstId";
        private const string KeyLastTradeId = "lastId";
        private const string KeyTradeCount = "count";

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
                new JProperty(KeySymbol, statistics.Symbol),
                new JProperty(KeyPriceChange, statistics.PriceChange.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyPriceChangePercent, statistics.PriceChangePercent.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyWeightedAveragePrice, statistics.WeightedAveragePrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyPreviousClosePrice, statistics.PreviousClosePrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyLastPrice, statistics.LastPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyLastQuantity, statistics.LastQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyBidPrice, statistics.BidPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyBidQuantity, statistics.BidQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyAskPrice, statistics.AskPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyAskQuantity, statistics.AskQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyOpenPrice, statistics.OpenPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyHighPrice, statistics.HighPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyLowPrice, statistics.LowPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyVolume, statistics.Volume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyQuoteVolume, statistics.QuoteVolume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyOpenTime, statistics.OpenTime.ToTimestamp()),
                new JProperty(KeyCloseTime, statistics.CloseTime.ToTimestamp()),
                new JProperty(KeyFirstTradeId, statistics.FirstTradeId),
                new JProperty(KeyLastTradeId, statistics.LastTradeId),
                new JProperty(KeyTradeCount, statistics.TradeCount)
            };

            return jObject.ToString(Formatting.None);
        }

        private static SymbolStatistics DeserializeSymbolStatistics(JToken jToken)
        {
            return new SymbolStatistics(
                jToken[KeySymbol].Value<string>(),
                TimeSpan.FromHours(24),
                jToken[KeyPriceChange].Value<decimal>(),
                jToken[KeyPriceChangePercent].Value<decimal>(),
                jToken[KeyWeightedAveragePrice].Value<decimal>(),
                jToken[KeyPreviousClosePrice].Value<decimal>(),
                jToken[KeyLastPrice].Value<decimal>(),
                jToken[KeyLastQuantity].Value<decimal>(),
                jToken[KeyBidPrice].Value<decimal>(),
                jToken[KeyBidQuantity].Value<decimal>(),
                jToken[KeyAskPrice].Value<decimal>(),
                jToken[KeyAskQuantity].Value<decimal>(),
                jToken[KeyOpenPrice].Value<decimal>(),
                jToken[KeyHighPrice].Value<decimal>(),
                jToken[KeyLowPrice].Value<decimal>(),
                jToken[KeyVolume].Value<decimal>(),
                jToken[KeyQuoteVolume].Value<decimal>(),
                jToken[KeyOpenTime].Value<long>().ToDateTime(),
                jToken[KeyCloseTime].Value<long>().ToDateTime(),
                jToken[KeyFirstTradeId].Value<long>(),
                jToken[KeyLastTradeId].Value<long>(),
                jToken[KeyTradeCount].Value<long>());
        }
    }
}
