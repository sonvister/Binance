using System;
using Xunit;

namespace Binance.Tests.Trades
{
    public class CandlestickTest
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            var interval = KlineInterval.Hour;
            var openTime = 1234567890;
            var open = 4950;
            var high = 5100;
            var low = 4900;
            var close = 5050;
            var volume = 1000;
            var closeTime = 2345678901;
            var quoteAssetVolume = 5000000;
            var numberOfTrades = 555555;
            var takerBuyBaseAssetVolume = 4444;
            var takerBuyQuoteAssetVolume = 333;

            Assert.Throws<ArgumentNullException>("symbol", () => new Candlestick(null, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));

            Assert.Throws<ArgumentException>("openTime", () => new Candlestick(symbol, interval, -1, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("openTime", () => new Candlestick(symbol, interval, 0, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));

            Assert.Throws<ArgumentException>("open", () => new Candlestick(symbol, interval, openTime, -1, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("high", () => new Candlestick(symbol, interval, openTime, open, -1, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("low", () => new Candlestick(symbol, interval, openTime, open, high, -1, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("close", () => new Candlestick(symbol, interval, openTime, open, high, low, -1, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));

            Assert.Throws<ArgumentException>("volume", () => new Candlestick(symbol, interval, openTime, open, high, low, close, -1, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("quoteAssetVolume", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, -1, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("takerBuyBaseAssetVolume", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, -1, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("takerBuyQuoteAssetVolume", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, -1));

            Assert.Throws<ArgumentException>("numberOfTrades", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, -1, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));

            Assert.Throws<ArgumentException>("closeTime", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, -1, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
            Assert.Throws<ArgumentException>("closeTime", () => new Candlestick(symbol, interval, openTime, open, high, low, close, volume, 0, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            var interval = KlineInterval.Hour;
            var openTime = 1234567890;
            var open = 4950;
            var high = 5100;
            var low = 4900;
            var close = 5050;
            var volume = 1000;
            var closeTime = 2345678901;
            var quoteAssetVolume = 5000000;
            var numberOfTrades = 555555;
            var takerBuyBaseAssetVolume = 4444;
            var takerBuyQuoteAssetVolume = 333;

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
    }
}
