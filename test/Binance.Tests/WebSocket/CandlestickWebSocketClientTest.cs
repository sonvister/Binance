using System;
using System.Linq;
using Binance.Client;
using Binance.Market;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    public class CandlestickWebSocketClientTest
    {
        [Fact]
        public void Throws()
        {
            var client = new CandlestickWebSocketClient();

            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe((string)null, CandlestickInterval.Hour));
            Assert.Throws<ArgumentNullException>("symbol", () => client.Subscribe(string.Empty, CandlestickInterval.Hour));
        }

        [Fact]
        public void Subscribe()
        {
            var client = new CandlestickWebSocketClient();

            Assert.Empty(client.ObservedStreams);
            Assert.Empty(client.Stream.ProvidedStreams);

            client.Subscribe(Symbol.BTC_USDT, CandlestickInterval.Hour);

            Assert.True(client.ObservedStreams.Count() == 1);
            Assert.True(client.Stream.ProvidedStreams.Count() == 1);
        }
    }
}
