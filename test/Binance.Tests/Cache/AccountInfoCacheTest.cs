using System;
using System.Linq;
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
            var client = new Mock<IUserDataClient>().Object;

            var cache = new AccountInfoCache(api, client);

            Assert.Throws<ArgumentNullException>("listenKey", () => cache.Subscribe(null, user));
            Assert.Throws<ArgumentNullException>("listenKey", () => cache.Subscribe(string.Empty, user));
            Assert.Throws<ArgumentNullException>("user", () => cache.Subscribe(listenKey, null));

            cache.Subscribe(listenKey, user);

            Assert.Throws<InvalidOperationException>(() => cache.Subscribe(listenKey, user));
        }

        [Fact]
        public void Unsubscribe()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IUserDataClient>().Object;

            var cache = new AccountInfoCache(api, client);

            // Can call unsubscribe before subscribe or multiple times without fail.
            cache.Unsubscribe();
            cache.Unsubscribe();
        }

        [Fact]
        public void LinkToClient()
        {
            var api = new Mock<IBinanceApi>().Object;
            var user1 = new Mock<IBinanceApiUser>().Object;
            var user2 = new Mock<IBinanceApiUser>().Object;
            var client1 = new Mock<IUserDataClient>().Object;
            var client2 = new UserDataClient();
            var listenKey1 = "<listen key 1>";
            var listenKey2 = "<listen key 2>";

            client2.Subscribe(listenKey1, user1);

            var clientSubscribeStreams = client2.SubscribedStreams.ToArray();
            Assert.Equal(listenKey1, clientSubscribeStreams.Single());

            var cache = new AccountInfoCache(api, client1)
            {
                Client = client2 // link client.
            };

            // Client subscribed streams are unchanged after link to unsubscribed cache.
            Assert.Equal(clientSubscribeStreams, client2.SubscribedStreams);

            cache.Client = client1; // unlink client.

            // Subscribe cache to listen key.
            cache.Subscribe(listenKey2, user2);

            // Cache is subscribed to listen key.
            Assert.Equal(listenKey2, cache.SubscribedStreams.Single());

            cache.Client = client2; // link to client.

            // Client has second subscribed stream from cache.
            Assert.True(client2.SubscribedStreams.Count() == 2);
            Assert.Contains(listenKey2, client2.SubscribedStreams);
        }
    }
}
