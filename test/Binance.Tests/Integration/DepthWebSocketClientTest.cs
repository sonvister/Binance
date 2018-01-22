//#define INTEGRATION

#if INTEGRATION

using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Integration
{
    public class DepthWebSocketClientTest
    {
        [Fact]
        public Task SubscribeThrows()
        {
            var client = new DepthWebSocketClient(new Mock<IWebSocketStream>().Object);

            using (var cts = new CancellationTokenSource())
                return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => client.StreamAsync(null, cts.Token));
        }

        [Fact]
        public async Task Properties()
        {
            var symbol = Symbol.BTC_USDT;
            var client = new DepthWebSocketClient(new BinanceWebSocketStream());

            using (var cts = new CancellationTokenSource())
            {
                var task = client.StreamAsync(symbol, cts.Token);

                Assert.Equal(symbol, client.Symbol);

                cts.Cancel();
                await task;
            }
        }

        [Fact]
        public Task SubscribeTwiceThrows()
        {
            var symbol = Symbol.BTC_USDT;
            var client = new DepthWebSocketClient(new BinanceWebSocketStream());

            using (var cts = new CancellationTokenSource())
            {
                client.StreamAsync(symbol, cts.Token);

                return Assert.ThrowsAsync<InvalidOperationException>(() => client.StreamAsync(symbol, cts.Token));
            }
        }

        [Fact]
        public async Task SubscribeCallback()
        {
            var symbol = Symbol.BTC_USDT;
            var client = new DepthWebSocketClient(new BinanceWebSocketStream());

            using (var cts = new CancellationTokenSource())
            {
                var task = client.StreamAsync(symbol, args =>
                {
                    // NOTE: The first event will cancel the client ...could be a while.
                    // ReSharper disable once AccessToDisposedClosure
                    cts.Cancel();
                },
                cts.Token);

                await task;
            }
        }
    }
}

#endif
