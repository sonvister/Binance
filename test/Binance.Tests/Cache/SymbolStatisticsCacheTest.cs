using System;
using Binance.Api;
using Binance.Cache;
using Binance.Client;
using Binance.WebSocket;
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
        public void LinkClient()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client1 = new Mock<ISymbolStatisticsClient>().Object;
            var client2 = new SymbolStatisticsWebSocketClient();

            var cache = new SymbolStatisticsCache(api, client1);

            Assert.Empty(client2.ObservedStreams);

            cache.Client = client2;

            Assert.Empty(client2.ObservedStreams);
        }
    }
}
