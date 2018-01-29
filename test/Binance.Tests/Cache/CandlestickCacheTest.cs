using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Cache;
using Binance.Market;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class CandlestickCacheTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ICandlestickWebSocketClient>().Object;

            var cache = new CandlestickCache(api, client);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAndStreamAsync(null, CandlestickInterval.Day, cts.Token));
            }
        }
    }
}
