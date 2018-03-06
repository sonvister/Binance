using System;
using Binance.Cache;
using Binance.Client;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class AccountInfoCacheTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var listenKey = "<valid listen key>";
            var api = new Mock<IBinanceApi>().Object;
            var user = new Mock<IBinanceApiUser>().Object;

            var cache = new AccountInfoCache(api, new Mock<IUserDataClient>().Object);

            Assert.Throws<ArgumentNullException>("listenKey", () => cache.Subscribe(null, user));
            Assert.Throws<ArgumentNullException>("listenKey", () => cache.Subscribe(string.Empty, user));
            Assert.Throws<ArgumentNullException>("user", () => cache.Subscribe(listenKey, null));

            cache.Subscribe(listenKey, user);

            Assert.Throws<InvalidOperationException>(() => cache.Subscribe(listenKey, user));
        }
    }
}
