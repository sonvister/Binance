using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket;
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
        public async Task StreamAsync()
        {
            var uri = new Uri("wss://stream.binance.com:9443");

            var client = new DefaultWebSocketClient();

            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();
                await client.StreamAsync(uri, cts.Token);
            }
        }
    }
}
