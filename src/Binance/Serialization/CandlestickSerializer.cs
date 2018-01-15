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
        private const string Key_Symbol = "symbol";
        private const string Key_Interval = "interval";
        private const string Key_OpenTime = "openTime";
        private const string Key_Open = "open";
        private const string Key_High = "high";
        private const string Key_Low = "low";
        private const string Key_Close = "close";
        private const string Key_Volume = "volume";
        private const string Key_CloseTime = "closeTime";
        private const string Key_QuoteAssetVolume = "quoteAssetVolume";
        private const string Key_NumberOfTrades = "numberOfTrades";
        private const string Key_TakerBuyBaseAssetVolume = "takerBuyBaseAssetVolume";
        private const string Key_TakerBuyQuoteAssetVolume = "takerBuyQuoteAssetVolume";

        public virtual Candlestick Deserialize(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            var jToken = JToken.Parse(json);

            return new Candlestick(
                jToken[Key_Symbol]?.Value<string>(),
                jToken[Key_Interval].Value<string>().ToCandlestickInterval(),
                jToken[Key_OpenTime].Value<long>(),
                jToken[Key_Open].Value<decimal>(),
                jToken[Key_High].Value<decimal>(),
                jToken[Key_Low].Value<decimal>(),
                jToken[Key_Close].Value<decimal>(),
                jToken[Key_Volume].Value<decimal>(),
                jToken[Key_CloseTime].Value<long>(),
                jToken[Key_QuoteAssetVolume].Value<decimal>(),
                jToken[Key_NumberOfTrades].Value<long>(),
                jToken[Key_TakerBuyBaseAssetVolume].Value<decimal>(),
                jToken[Key_TakerBuyQuoteAssetVolume].Value<decimal>());
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
                new JProperty(Key_Symbol, candlestick.Symbol),
                new JProperty(Key_Interval, candlestick.Interval.AsString()),
                new JProperty(Key_OpenTime, candlestick.OpenTime),
                new JProperty(Key_Open, candlestick.Open.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_High, candlestick.High.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Low, candlestick.Low.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Close, candlestick.Close.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Volume, candlestick.Volume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_CloseTime, candlestick.CloseTime),
                new JProperty(Key_QuoteAssetVolume, candlestick.QuoteAssetVolume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_NumberOfTrades, candlestick.NumberOfTrades),
                new JProperty(Key_TakerBuyBaseAssetVolume, candlestick.TakerBuyBaseAssetVolume.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_TakerBuyQuoteAssetVolume, candlestick.TakerBuyQuoteAssetVolume.ToString(CultureInfo.InvariantCulture))
            };

            return jObject.ToString(Formatting.None);
        }
    }
}
