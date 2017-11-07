using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket;
using Binance.Market;
using Xunit;

namespace Binance.Tests.Api.WebSocket
{
    public class CandlesticksWebSocketClientTest
    {
        [Fact]
        public Task SubscribeThrows()
        {
            using (var cts = new CancellationTokenSource())
            using (var client = new CandlestickWebSocketClient())
                return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.SubscribeAsync(null, CandlestickInterval.Hour, cts.Token));
        }
    }
}
