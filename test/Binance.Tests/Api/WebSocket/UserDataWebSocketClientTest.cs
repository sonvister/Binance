using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Api.WebSocket
{
    public class UserDataWebSocketClientTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var webSocket = new Mock<IWebSocketStream>();
            webSocket.SetupGet(_ => _.Client).Returns(new Mock<IWebSocketClient>().Object);

            var client = new UserDataWebSocketClient(new Mock<IBinanceApi>().Object, webSocket.Object);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => client.StreamAsync(null, cts.Token));
            }
        }
    }
}
