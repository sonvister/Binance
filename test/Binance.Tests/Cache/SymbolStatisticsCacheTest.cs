using System;
using System.Linq;
using Binance.Cache;
using Binance.Client;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class SymbolStatisticsCacheTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ISymbolStatisticsClient>().Object;

            var cache = new SymbolStatisticsCache(api, client);

            cache.Subscribe();

            Assert.Throws<InvalidOperationException>(() => cache.Subscribe());
        }

        [Fact]
        public void Unsubscribe()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ISymbolStatisticsClient>().Object;

            var cache = new SymbolStatisticsCache(api, client);

            // Can call unsubscribe before subscribe or multiple times without fail.
            cache.Unsubscribe();
            cache.Unsubscribe();
        }

        [Fact]
        public void LinkToClient()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client1 = new Mock<ISymbolStatisticsClient>().Object;
            var client2 = new SymbolStatisticsClient();
            var symbol1 = Symbol.BTC_USDT;
            var symbol2 = Symbol.LTC_BTC;

            client2.Subscribe(symbol1);

            var clientSubscribeStreams = client2.SubscribedStreams.ToArray();
            Assert.Equal(SymbolStatisticsClient.GetStreamName(symbol1), clientSubscribeStreams.Single());

            var cache = new SymbolStatisticsCache(api, client1)
            {
                Client = client2 // link client.
            };

            // Client subscribed streams are unchanged after link to unsubscribed cache.
            Assert.Equal(clientSubscribeStreams, client2.SubscribedStreams);

            cache.Client = client1; // unlink client.

            // Subscribe cache to symbol.
            cache.Subscribe(symbol2);

            // Cache is subscribed to symbol.
            Assert.Equal(SymbolStatisticsClient.GetStreamName(symbol2), cache.SubscribedStreams.Single());

            cache.Client = client2; // link to client.

            // Client has second subscribed stream from cache.
            Assert.True(client2.SubscribedStreams.Count() == 2);
            Assert.Contains(SymbolStatisticsClient.GetStreamName(symbol2), client2.SubscribedStreams);
        }
    }
}
