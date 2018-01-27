//#define INTEGRATION

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
        public void SubscribeThrows()
        {
            var client = new DepthWebSocketClient(new Mock<IWebSocketStream>().Object);

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(null));
        }

        [Fact]
        public void SubscribeTwiceIgnored()
        {
            var symbol = Symbol.LTC_USDT;
            var client = new DepthWebSocketClient(new BinanceWebSocketStream());

            client.Subscribe(symbol);
            client.Subscribe(symbol);
        }

#if INTEGRATION
        [Fact]
        public async Task SubscribeCallback()
        {
            var symbol = Symbol.LTC_USDT;
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
#endif
    }
}

