using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket;
using Xunit;

namespace Binance.Tests.Api.WebSocket
{
    public class TradesWebSocketClientTest
    {
        [Fact]
        public Task SubscribeThrows()
        {
            using (var cts = new CancellationTokenSource())
            using (var client = new TradesWebSocketClient())
                return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.SubscribeAsync(null, cts.Token));
        }
    }
}
