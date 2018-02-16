using System;
using Binance.Market;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class CandlesticksWebSocketClientTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var client = new CandlestickWebSocketClient();

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(null, CandlestickInterval.Hour));
        }
    }
}
