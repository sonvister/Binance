using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Api.WebSocket
{
    public class DepthWebSocketClientTest
    {
        [Fact]
        public async Task SubscribeThrows()
        {
            var client = new DepthWebSocketClient(new Mock<IWebSocketClient>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.SubscribeAsync(null, new CancellationToken()));
            await Assert.ThrowsAsync<ArgumentException>("token", () => client.SubscribeAsync(Symbol.BTC_USDT, CancellationToken.None));
        }
    }
}
