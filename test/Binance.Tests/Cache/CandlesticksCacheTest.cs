using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Binance.Market;
using Moq;
using System;
using Xunit;

namespace Binance.Tests.Cache
{
    public class CandlesticksCacheTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ICandlestickWebSocketClient>().Object;

            var cache = new CandlesticksCache(api, client);

            Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAsync(null, CandlestickInterval.Day));
        }
    }
}
