using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Api.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Api.WebSocket
{
    public class UserDataWebSocketClientTest
    {
        [Fact]
        public Task SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;

            using (var cts = new CancellationTokenSource())
            using (var client = new UserDataWebSocketClient(api))
                return Assert.ThrowsAsync<ArgumentNullException>("user", () => client.SubscribeAsync(null, cts.Token));
        }
    }
}
