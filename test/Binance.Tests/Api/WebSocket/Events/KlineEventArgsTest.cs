using System;
using Binance.Api.WebSocket.Events;
using Binance.Market;
using Xunit;

namespace Binance.Tests.Api.WebSocket.Events
{
    public class KlineEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const long firstTradeId = 1234567890;
            const long lastTradeId = 1234567899;
            const bool isFinal = true;

            var symbol = Symbol.BTC_USDT;
            const KlineInterval interval = KlineInterval.Hour;
            const long openTime = 1234567890;
            const decimal open = 4950;
            const decimal high = 5100;
            const decimal low = 4900;
            const decimal close = 5050;
            const decimal volume = 1000;
            const long closeTime = 2345678901;
            const long quoteAssetVolume = 5000000;
            const int numberOfTrades = 555555;
            const decimal takerBuyBaseAssetVolume = 4444;
            const decimal takerBuyQuoteAssetVolume = 333;

            var candlestick = new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume);

            Assert.Throws<ArgumentException>("timestamp", () => new KlineEventArgs(-1, candlestick, firstTradeId, lastTradeId, isFinal));
            Assert.Throws<ArgumentException>("timestamp", () => new KlineEventArgs(0, null, firstTradeId, lastTradeId, isFinal));
            Assert.Throws<ArgumentNullException>("candlestick", () => new KlineEventArgs(timestamp, null, firstTradeId, lastTradeId, isFinal));
            Assert.Throws<ArgumentException>("firstTradeId", () => new KlineEventArgs(timestamp, candlestick, -1, lastTradeId, isFinal));
            Assert.Throws<ArgumentException>("lastTradeId", () => new KlineEventArgs(timestamp, candlestick, firstTradeId, -1, isFinal));
            Assert.Throws<ArgumentException>("lastTradeId", () => new KlineEventArgs(timestamp, candlestick, firstTradeId, firstTradeId - 1, isFinal));
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const long firstTradeId = 1234567890;
            const long lastTradeId = 1234567899;
            const bool isFinal = true;

            var symbol = Symbol.BTC_USDT;
            const KlineInterval interval = KlineInterval.Hour;
            const long openTime = 1234567890;
            const decimal open = 4950;
            const decimal high = 5100;
            const decimal low = 4900;
            const decimal close = 5050;
            const decimal volume = 1000;
            const long closeTime = 2345678901;
            const long quoteAssetVolume = 5000000;
            const int numberOfTrades = 555555;
            const decimal takerBuyBaseAssetVolume = 4444;
            const decimal takerBuyQuoteAssetVolume = 333;

            var candlestick = new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume);

            var args = new KlineEventArgs(timestamp, candlestick, firstTradeId, lastTradeId, isFinal);

            Assert.Equal(timestamp, args.Timestamp);
            Assert.Equal(candlestick, args.Candlestick);
            Assert.Equal(firstTradeId, args.FirstTradeId);
            Assert.Equal(lastTradeId, args.LastTradeId);
            Assert.Equal(isFinal, args.IsFinal);
        }
    }
}
