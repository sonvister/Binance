using System;
using Binance.Client;
using Binance.Market;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class CandlestickWebSocketClientTest
    {
        [Fact]
        public void Throws()
        {
            var client = new CandlestickWebSocketClient();

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe((string)null, CandlestickInterval.Hour));
            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(string.Empty, CandlestickInterval.Hour));
        }
    }
}
