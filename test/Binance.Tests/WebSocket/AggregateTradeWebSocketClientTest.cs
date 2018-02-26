using System;
using Binance.Client;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class AggregateTradeWebSocketClientTest
    {
        [Fact]
        public void Throws()
        {
            var client = new AggregateTradeWebSocketClient();

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe((string)null));
            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(string.Empty));
        }
    }
}
