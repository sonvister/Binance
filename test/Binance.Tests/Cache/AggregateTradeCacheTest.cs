using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Moq;
using System;
using Xunit;
using System.Threading.Tasks;
using System.Threading;

namespace Binance.Tests.Cache
{
    public class AggregateTradeCacheTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IAggregateTradeWebSocketClient>().Object;

            var cache = new AggregateTradeCache(api, client);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.StreamAsync(null, cts.Token));
            }
        }
    }
}
