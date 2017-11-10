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
    public class AccountInfoCacheTest
    {
        [Fact]
        public async Task SubscribeThrows()
        {
            var user = new BinanceApiUser("api-key");
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IUserDataWebSocketClient>().Object;

            var cache = new AccountInfoCache(api, client);

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => cache.SubscribeAsync(null, new CancellationToken()));
            await Assert.ThrowsAsync<ArgumentException>("token", () => cache.SubscribeAsync(user, CancellationToken.None));
        }
    }
}
