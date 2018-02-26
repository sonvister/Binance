using System;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class SymbolStatisticsWebSocketClientTest
    {
        [Fact]
        public void Throws()
        {
            var client = new SymbolStatisticsWebSocketClient();

            Assert.Throws<ArgumentException>("symbols", () => client.Subscribe(null));
        }
    }
}
