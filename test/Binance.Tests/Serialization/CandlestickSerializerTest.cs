using System;
using Binance.Market;
using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class CandlestickSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var symbol = Symbol.BTC_USDT;
            const CandlestickInterval interval = CandlestickInterval.Hour;
            var openTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const decimal open = 4950;
            const decimal high = 5100;
            const decimal low = 4900;
            const decimal close = 5050;
            const decimal volume = 1000;
            var closeTime = DateTimeOffset.FromUnixTimeMilliseconds(openTime).AddHours(1).ToUnixTimeMilliseconds();
            const long quoteAssetVolume = 5000000;
            const int numberOfTrades = 555555;
            const decimal takerBuyBaseAssetVolume = 4444;
            const decimal takerBuyQuoteAssetVolume = 333;

            var candlestick = new Candlestick(symbol, interval, openTime, open, high, low, close, volume, closeTime, quoteAssetVolume, numberOfTrades, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume);

            var serializer = new CandlestickSerializer();

            var json = serializer.Serialize(candlestick);

            var other = serializer.Deserialize(json);

            Assert.True(candlestick.Equals(other));
        }
    }
}
