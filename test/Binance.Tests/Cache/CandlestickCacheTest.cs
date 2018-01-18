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
        public async Task StreamThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ICandlestickWebSocketClient>().Object;

            var cache = new CandlestickCache(api, client);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.StreamAsync(null, CandlestickInterval.Day, cts.Token));
            }
        }
    }
}
