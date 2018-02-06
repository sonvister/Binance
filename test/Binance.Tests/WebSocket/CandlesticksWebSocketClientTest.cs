using System;
using Binance.Market;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class CandlesticksWebSocketClientTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var client = new CandlestickWebSocketClient(new Mock<IWebSocketStream>().Object);

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(null, CandlestickInterval.Hour));
        }
    }
}
