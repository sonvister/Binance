using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Cache;
using Binance.WebSocket.UserData;
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

            var cache = new AccountInfoCache(api, new Mock<IUserDataWebSocketManager>().Object);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => cache.SubscribeAndStreamAsync(null, cts.Token));
            }
        }
    }
}
