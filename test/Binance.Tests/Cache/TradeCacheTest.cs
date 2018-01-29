using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Cache;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class TradeCacheTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ITradeWebSocketClient>().Object;

            var cache = new TradeCache(api, client);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAndStreamAsync(null, cts.Token));
            }
        }
    }
}
