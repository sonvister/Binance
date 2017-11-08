using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Api.WebSocket
{
    public class TradesWebSocketClientTest
    {
        [Fact]
        public Task SubscribeThrows()
        {
            using (var cts = new CancellationTokenSource())
            using (var client = new TradesWebSocketClient(new Mock<IWebSocketClient>().Object))
                return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.SubscribeAsync(null, cts.Token));
        }
    }
}
