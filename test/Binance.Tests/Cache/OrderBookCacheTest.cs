using System;
using System.Linq;
using Binance.Cache;
using Binance.Client;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class OrderBookCacheTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var symbol = Symbol.BTC_USDT;
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IDepthClient>().Object;

            var cache = new OrderBookCache(api, client);

            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(null));
            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(string.Empty));

            cache.Subscribe(symbol);

            Assert.Throws<InvalidOperationException>(() => cache.Subscribe(symbol));
        }

        [Fact]
        public void Unsubscribe()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IDepthClient>().Object;

            var cache = new OrderBookCache(api, client);

            // Can call unsubscribe before subscribe or multiple times without fail.
            cache.Unsubscribe();
            cache.Unsubscribe();
        }

        [Fact]
        public void LinkToClient()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client1 = new Mock<IDepthClient>().Object;
            var client2 = new DepthClient();
            var symbol1 = Symbol.BTC_USDT;
            var symbol2 = Symbol.LTC_BTC;

            client2.Subscribe(symbol1);

            var clientSubscribeStreams = client2.SubscribedStreams.ToArray();
            Assert.Equal(DepthClient.GetStreamName(symbol1), clientSubscribeStreams.Single());

            var cache = new OrderBookCache(api, client1)
            {
                Client = client2 // link client.
            };

            // Client subscribed streams are unchanged after link to unsubscribed cache.
            Assert.Equal(clientSubscribeStreams, client2.SubscribedStreams);

            cache.Client = client1; // unlink client.

            // Subscribe cache to symbol.
            cache.Subscribe(symbol2);

            // Cache is subscribed to symbol.
            Assert.Equal(DepthClient.GetStreamName(symbol2), cache.SubscribedStreams.Single());

            cache.Client = client2; // link to client.

            // Client has second subscribed stream from cache.
            Assert.True(client2.SubscribedStreams.Count() == 2);
            Assert.Contains(DepthClient.GetStreamName(symbol2), client2.SubscribedStreams);
        }
    }
}
