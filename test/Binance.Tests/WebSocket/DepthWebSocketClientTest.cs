using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class DepthWebSocketClientTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var client = new DepthWebSocketClient(new Mock<IWebSocketStream>().Object);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.SubscribeAndStreamAsync(null, cts.Token));
            }
        }
    }
}
