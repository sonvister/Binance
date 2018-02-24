using System;
using Binance.Client;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class TradesWebSocketClientTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var client = new TradeWebSocketClient();

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe((string)null));
        }
    }
}
