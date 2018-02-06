using System;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class DepthWebSocketClientTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var client = new DepthWebSocketClient(new Mock<IWebSocketStream>().Object);

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(null));
        }
    }
}
