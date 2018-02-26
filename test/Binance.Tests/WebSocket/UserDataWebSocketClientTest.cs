using System;
using System.Linq;
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

        [Fact]
        public void Subscribe()
        {
            var listenKey = "<valid listen key>";
            var user = new Mock<IBinanceApiUser>().Object;
            var client = new UserDataWebSocketClient();

            Assert.Empty(client.ObservedStreams);
            Assert.Empty(client.Stream.ProvidedStreams);

            client.Subscribe(listenKey, user);

            Assert.True(client.ObservedStreams.Count() == 1);
            Assert.True(client.Stream.ProvidedStreams.Count() == 1);
        }
    }
}
