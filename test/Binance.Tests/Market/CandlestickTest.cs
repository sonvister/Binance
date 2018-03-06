using System;
using Binance.Serialization;
using Newtonsoft.Json;
using Xunit;

namespace Binance.Tests.Market
{
    public class CandlestickTest
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            const CandlestickInterval interval = CandlestickInterval.Hour;
            var openTime = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const decimal open = 4950;
            const decimal high = 5100;
            const decimal low = 4900;
            const decimal close = 5050;
            const decimal volume = 1000;
            var closeTime = openTime.AddHours(1);
            const long quoteAssetVolume = 5000000;
            const int numberOfTrades = 555555;
            const decimal takerBuyBaseAssetVolume = 4444;
            const decimal takerBuyQuoteAssetVolume = 333;

            Assert.Throws<ArgumentNullException>("symbol", () => new Candlestick(null, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));

            Assert.Throws<ArgumentException>("open", () => new Candlestick(symbol, interval, openTime, -1, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("high", () => new Candlestick(symbol, interval, openTime, open, -1, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("low", () => new Candlestick(symbol, interval, openTime, open, high, -1, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("close", () => new Candlestick(symbol, interval, openTime, open, high, low, -1, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));

            Assert.Throws<ArgumentException>("volume", () => new Candlestick(symbol, interval, openTime, open, high, low, close, -1, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("quoteAssetVolume", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, -1, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("takerBuyBaseAssetVolume", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, -1, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("takerBuyQuoteAssetVolume", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, -1));

            Assert.Throws<ArgumentException>("numberOfTrades", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, -1, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            const CandlestickInterval interval = CandlestickInterval.Hour;
            var openTime = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const decimal open = 4950;
            const decimal high = 5100;
            const decimal low = 4900;
            const decimal close = 5050;
            const decimal volume = 1000;
            var closeTime = openTime.AddHours(1);
            const long quoteAssetVolume = 5000000;
            const int numberOfTrades = 555555;
            const decimal takerBuyBaseAssetVolume = 4444;
            const decimal takerBuyQuoteAssetVolume = 333;

            var candlestick = new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume);

            Assert.Equal(symbol, candlestick.Symbol);
            Assert.Equal(interval, candlestick.Interval);
            Assert.Equal(openTime, candlestick.OpenTime);
            Assert.Equal(open, candlestick.Open);
            Assert.Equal(high, candlestick.High);
            Assert.Equal(low, candlestick.Low);
            Assert.Equal(close, candlestick.Close);
            Assert.Equal(volume, candlestick.Volume);
            Assert.Equal(closeTime, candlestick.CloseTime);
            Assert.Equal(quoteAssetVolume, candlestick.QuoteAssetVolume);
            Assert.Equal(numberOfTrades, candlestick.NumberOfTrades);
            Assert.Equal(takerBuyBaseAssetVolume, candlestick.TakerBuyBaseAssetVolume);
            Assert.Equal(takerBuyQuoteAssetVolume, candlestick.TakerBuyQuoteAssetVolume);
        }

        [Fact]
        public void Serialization()
        {
            var symbol = Symbol.BTC_USDT;
            const CandlestickInterval interval = CandlestickInterval.Hour;
            var openTime = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const decimal open = 4950;
            const decimal high = 5100;
            const decimal low = 4900;
            const decimal close = 5050;
            const decimal volume = 1000;
            var closeTime = openTime.AddHours(1);
            const long quoteAssetVolume = 5000000;
            const int numberOfTrades = 555555;
            const decimal takerBuyBaseAssetVolume = 4444;
            const decimal takerBuyQuoteAssetVolume = 333;

            var candlestick = new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new TimestampJsonConverter());

            var json = JsonConvert.SerializeObject(candlestick, settings);

            candlestick = JsonConvert.DeserializeObject<Candlestick>(json, settings);

            Assert.Equal(symbol, candlestick.Symbol);
            Assert.Equal(interval, candlestick.Interval);
            Assert.Equal(openTime, candlestick.OpenTime);
            Assert.Equal(open, candlestick.Open);
            Assert.Equal(high, candlestick.High);
            Assert.Equal(low, candlestick.Low);
            Assert.Equal(close, candlestick.Close);
            Assert.Equal(volume, candlestick.Volume);
            Assert.Equal(closeTime, candlestick.CloseTime);
            Assert.Equal(quoteAssetVolume, candlestick.QuoteAssetVolume);
            Assert.Equal(numberOfTrades, candlestick.NumberOfTrades);
            Assert.Equal(takerBuyBaseAssetVolume, candlestick.TakerBuyBaseAssetVolume);
            Assert.Equal(takerBuyQuoteAssetVolume, candlestick.TakerBuyQuoteAssetVolume);
        }
    }
}
