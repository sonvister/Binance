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
        public async Task SubscribeThrows()
        {
            var user = new BinanceApiUser("api-key");
            var client = new UserDataWebSocketClient(new Mock<IBinanceApi>().Object, new Mock<IWebSocketClient>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => client.SubscribeAsync(null, new CancellationToken()));
            await Assert.ThrowsAsync<ArgumentException>("token", () => client.SubscribeAsync(user, CancellationToken.None));
        }
    }
}
