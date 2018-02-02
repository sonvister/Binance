using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket;
using Binance.WebSocket.UserData;
using Moq;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class UserDataWebSocketManagerTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var client = new UserDataWebSocketManager(new Mock<IBinanceApi>().Object, new WebSocketStreamProvider(), new UserDataKeepAliveTimerProvider());

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => client.SubscribeAndStreamAsync(null, cts.Token));
            }
        }
    }
}
