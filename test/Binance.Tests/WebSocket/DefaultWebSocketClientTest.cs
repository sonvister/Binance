using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class DefaultWebSocketClientTest
    {
        [Fact]
        public async Task Throws()
        {
            var uri = new Uri("wss://stream.binance.com:9443");

            var client = new DefaultWebSocketClient();

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("uri", () => client.StreamAsync(null, cts.Token));
                await Assert.ThrowsAsync<ArgumentException>("token", () => client.StreamAsync(uri, CancellationToken.None));
            }
        }

        [Fact]
        public async Task StreamCancel()
        {
            var uri = new Uri("wss://stream.binance.com:9443");

            var client = new DefaultWebSocketClient();

            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();
                await client.StreamAsync(uri, cts.Token);
            }
        }

        [Fact]
        public async Task StreamOpenEvent()
        {
            var uri = new Uri("wss://stream.binance.com:9443");

            var webSocket = new Mock<IClientWebSocket>();
            webSocket.Setup(w => w.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            webSocket.Setup(w => w.State).Returns(WebSocketState.Open);

            var factory = new Mock<IClientWebSocketFactory>();
            factory.Setup(f => f.CreateClientWebSocket()).Returns(webSocket.Object);

            var client = new DefaultWebSocketClient(factory.Object);

            using (var cts = new CancellationTokenSource())
            {
                client.Open += (s, e) =>
                {
                    cts.Cancel();
                };

                await client.StreamAsync(uri, cts.Token);
            }
        }
    }
}
