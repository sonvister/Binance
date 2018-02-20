using System;
using Binance.Client;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class DepthWebSocketClientTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var client = new DepthWebSocketClient();

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(null));
        }

        [Fact]
        public void SubscribeTwiceIgnored()
        {
            var symbol = Symbol.LTC_USDT;
            var client = new DepthWebSocketClient();

            client.Subscribe(symbol);
            client.Subscribe(symbol);
        }
    }
}
