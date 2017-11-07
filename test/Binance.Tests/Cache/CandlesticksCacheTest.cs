using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Binance.Market;
using Moq;
using System;
using Xunit;
using System.Threading.Tasks;

namespace Binance.Tests.Cache
{
    public class CandlesticksCacheTest
    {
        [Fact]
        public Task SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ICandlestickWebSocketClient>().Object;

            var cache = new CandlesticksCache(api, client);

            return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAsync(null, CandlestickInterval.Day));
        }
    }
}
