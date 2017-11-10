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
        public async Task SubscribeThrows()
        {
            var client = new CandlestickWebSocketClient(new Mock<IWebSocketClient>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.SubscribeAsync(null, CandlestickInterval.Hour, new CancellationToken()));
            await Assert.ThrowsAsync<ArgumentException>("token", () => client.SubscribeAsync(Symbol.BTC_USDT, CandlestickInterval.Hour, CancellationToken.None));
        }
    }
}
