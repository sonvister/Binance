using System;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class TradesWebSocketClientTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var client = new AggregateTradeWebSocketClient(new Mock<IWebSocketStream>().Object);

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(null));
        }
    }
}
