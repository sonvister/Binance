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
    public class OrderBookCacheTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IDepthWebSocketClient>().Object;

            var cache = new OrderBookCache(api, client);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.StreamAsync(null, cts.Token));
            }
        }
    }
}
