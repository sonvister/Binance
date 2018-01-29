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
    public class AccountInfoCacheTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var api = new Mock<IBinanceApi>().Object;

            var cache = new AccountInfoCache(api, new Mock<IUserDataWebSocketClient>().Object);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => cache.SubscribeAndStreamAsync(null, cts.Token));
            }
        }
    }
}
