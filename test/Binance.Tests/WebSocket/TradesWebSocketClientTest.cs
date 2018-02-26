using System;
using System.Linq;
using Binance.Client;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class TradesWebSocketClientTest
    {
        [Fact]
        public void Throws()
        {
            var client = new TradeWebSocketClient();

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe((string)null));
            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(string.Empty));
        }

        [Fact]
        public void Subscribe()
        {
            var client = new TradeWebSocketClient();

            Assert.Empty(client.ObservedStreams);
            Assert.Empty(client.Stream.ProvidedStreams);

            client.Subscribe(Symbol.BTC_USDT);

            Assert.True(client.ObservedStreams.Count() == 1);
            Assert.True(client.Stream.ProvidedStreams.Count() == 1);
        }
    }
}
