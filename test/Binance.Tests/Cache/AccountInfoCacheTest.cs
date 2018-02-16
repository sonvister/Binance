using System;
using Binance.Api;
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
            var api = new Mock<IBinanceApi>().Object;

            var cache = new AccountInfoCache(api, new Mock<IUserDataClient>().Object);

            Assert.Throws<ArgumentNullException>("listenKey", () => cache.Subscribe(null, null, null));
            Assert.Throws<ArgumentNullException>("user", () => cache.Subscribe("<valid listen key>", null, null));
        }
    }
}
