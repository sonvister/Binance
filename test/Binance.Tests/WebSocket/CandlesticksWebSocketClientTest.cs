using System;
using Binance.Client;
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

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe((string)null, CandlestickInterval.Hour));
        }
    }
}
