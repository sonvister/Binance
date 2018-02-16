using System;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class TradesWebSocketClientTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var client = new AggregateTradeWebSocketClient();

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(null));
        }
    }
}
