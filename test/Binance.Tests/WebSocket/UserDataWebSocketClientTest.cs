using System;
using Binance.Api;
using Binance.Client;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class UserDataWebSocketClientTest
    {
        [Fact]
        public void Throws()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var client = new UserDataWebSocketClient();

            Assert.Throws<ArgumentNullException>("listenKey", () => client.Subscribe(null, user));
            Assert.Throws<ArgumentNullException>("listenKey", () => client.Subscribe(string.Empty, user));
        }
    }
}
