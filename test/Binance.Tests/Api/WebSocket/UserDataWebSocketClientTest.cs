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
        public async Task StreamThrows()
        {
            var user = new BinanceApiUser("api-key");
            var client = new UserDataWebSocketClient(new Mock<IBinanceApi>().Object, new Mock<IWebSocketStream>().Object);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => client.StreamAsync(null, cts.Token));
            }
        }
    }
}
