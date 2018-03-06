using System;
using System.Linq;
using Binance.WebSocket;
using Binance.Client;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class SymbolStatisticsWebSocketClientTest
    {
        [Fact]
        public void Throws()
        {
            var client = new SymbolStatisticsWebSocketClient();

            Assert.Throws<ArgumentException>("symbols", () => client.Subscribe(null));
        }

        [Fact]
        public void Subscribe()
        {
            var client = new SymbolStatisticsWebSocketClient();

            Assert.Empty(client.SubscribedStreams);
            Assert.Empty(client.Publisher.PublishedStreams);

            client.Subscribe(Symbol.BTC_USDT);

            Assert.True(client.SubscribedStreams.Count() == 1);
            Assert.True(client.Publisher.PublishedStreams.Count() == 1);
        }
    }
}
