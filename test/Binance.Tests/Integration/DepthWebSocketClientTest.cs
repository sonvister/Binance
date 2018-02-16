//#define INTEGRATION

using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Integration
{
    public class DepthWebSocketClientTest
    {
#if INTEGRATION
        [Fact]
        public async Task SubscribeCallback()
        {
            var symbol = Symbol.LTC_USDT;
            var client = new DepthWebSocketClient(new BinanceWebSocketStream());

            using (var cts = new CancellationTokenSource())
            {
                var task = client.SubscribeAndStreamAsync(symbol, args =>
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

