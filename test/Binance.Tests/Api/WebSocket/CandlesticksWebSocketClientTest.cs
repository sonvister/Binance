using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket;
using Binance.Market;
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
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.StreamAsync(null, CandlestickInterval.Hour, cts.Token));
            }
        }
    }
}
