using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Binance.Market;
using Moq;
using System;
using Xunit;
using System.Threading.Tasks;
using System.Threading;

namespace Binance.Tests.Cache
{
    public class CandlestickCacheTest
    {
        [Fact]
        public async Task SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ICandlestickWebSocketClient>().Object;

            var cache = new CandlestickCache(api, client);

            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAsync(null, CandlestickInterval.Day, new CancellationToken()));
            await Assert.ThrowsAsync<ArgumentException>("token", () => cache.SubscribeAsync(Symbol.BTC_USDT, CandlestickInterval.Day, CancellationToken.None));
        }
    }
}
