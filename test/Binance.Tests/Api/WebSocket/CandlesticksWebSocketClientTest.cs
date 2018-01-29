using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Market;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Api.WebSocket
{
    public class CandlesticksWebSocketClientTest
    {
        [Fact]
        public async Task StreamThrows()
        {
            var client = new CandlestickWebSocketClient(new Mock<IWebSocketStream>().Object);

            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.SubscribeAndStreamAsync(null, CandlestickInterval.Hour, cts.Token));
            }
        }
    }
}
