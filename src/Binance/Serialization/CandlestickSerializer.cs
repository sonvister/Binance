using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binance.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class CandlestickSerializer : ICandlestickSerializer
    {
        private const string KeySymbol = "symbol";
        private const string KeyInterval = "interval";
        private const string KeyOpenTime = "openTime";
        private const string KeyOpen = "open";
        private const string KeyHigh = "high";
        private const string KeyLow = "low";
        private const string KeyClose = "close";
        private const string KeyVolume = "volume";
        private const string KeyCloseTime = "closeTime";
        private const string KeyQuoteAssetVolume = "quoteAssetVolume";
        private const string KeyNumberOfTrades = "numberOfTrades";
        private const string KeyTakerBuyBaseAssetVolume = "takerBuyBaseAssetVolume";
        private const string KeyTakerBuyQuoteAssetVolume = "takerBuyQuoteAssetVolume";

        public virtual Candlestick Deserialize(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            var jToken = JToken.Parse(json);

            return new Candlestick(
                jToken[KeySymbol]?.Value<string>(),
                jToken[KeyInterval].Value<string>().ToCandlestickInterval(),
                jToken[KeyOpenTime].Value<long>(),
                jToken[KeyOpen].Value<decimal>(),
                jToken[KeyHigh].Value<decimal>(),
                jToken[KeyLow].Value<decimal>(),
                jToken[KeyClose].Value<decimal>(),
                jToken[KeyVolume].Value<decimal>(),
                jToken[KeyCloseTime].Value<long>(),
                jToken[KeyQuoteAssetVolume].Value<decimal>(),
                jToken[KeyNumberOfTrades].Value<long>(),
                jToken[KeyTakerBuyBaseAssetVolume].Value<decimal>(),
                jToken[KeyTakerBuyQuoteAssetVolume].Value<decimal>());
        }

        public virtual IEnumerable<Candlestick> DeserializeMany(string json, string symbol, CandlestickInterval interval)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            return JArray.Parse(json).Select(item => new Candlestick(
                symbol, interval,
                item[0].Value<long>(),    // open time
                item[1].Value<decimal>(), // open
                item[2].Value<decimal>(), // high
                item[3].Value<decimal>(), // low
                item[4].Value<decimal>(), // close
                item[5].Value<decimal>(), // volume
                item[6].Value<long>(),    // close time
                item[7].Value<decimal>(), // quote asset volume
                item[8].Value<long>(),    // number of trades
                item[9].Value<decimal>(), // taker buy base asset volume
                item[10].Value<decimal>() // taker buy quote asset volume
            )).ToArray();
        }

        public virtual string Serialize(Candlestick candlestick)
        {
            Throw.IfNull(candlestick, nameof(candlestick));

            var jObject = new JObject
            {
                new JProperty(KeySymbol, candlestick.Symbol),
                new JProperty(KeyInterval, candlestick.Interval.AsString()),
                new JProperty(KeyOpenTime, candlestick.OpenTime),
                new JProperty(KeyOpen, candlestick.Open.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyHigh, candlestick.High.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyLow, candlestick.Low.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyClose, candlestick.Close.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyVolume, candlestick.Volume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyCloseTime, candlestick.CloseTime),
                new JProperty(KeyQuoteAssetVolume, candlestick.QuoteAssetVolume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyNumberOfTrades, candlestick.NumberOfTrades),
                new JProperty(KeyTakerBuyBaseAssetVolume, candlestick.TakerBuyBaseAssetVolume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyTakerBuyQuoteAssetVolume, candlestick.TakerBuyQuoteAssetVolume.ToString(CultureInfo.InvariantCulture))
            };

            return jObject.ToString(Formatting.None);
        }
    }
}
