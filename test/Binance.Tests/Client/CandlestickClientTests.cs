using System;
using System.Linq;
using Binance.Client;
using Xunit;

namespace Binance.Tests.Client
{
    public class CandlestickClientTests
    {
        private ICandlestickClient _client;

        public CandlestickClientTests()
        {
            _client = new CandlestickClient();
        }

        [Fact]
        public void Throws()
        {
            const CandlestickInterval interval = CandlestickInterval.Hour;

            Assert.Throws<ArgumentNullException>("symbol", () => _client.Subscribe((string)null, interval));
            Assert.Throws<ArgumentNullException>("symbol", () => _client.Subscribe(string.Empty, interval));

            Assert.Throws<ArgumentNullException>("symbol", () => _client.Unsubscribe((string)null, interval));
            Assert.Throws<ArgumentNullException>("symbol", () => _client.Unsubscribe(string.Empty, interval));
        }

        [Fact]
        public void Subscribe()
        {
            var symbol = Symbol.BTC_USDT;
            const CandlestickInterval interval = CandlestickInterval.Hour;

            Assert.Empty(_client.SubscribedStreams);

            // Subscribe to symbol.
            _client.Subscribe(symbol, interval);
            Assert.True(_client.SubscribedStreams.Count() == 1);

            // Re-Subscribe to same symbol doesn't fail.
            _client.Subscribe(symbol, interval);
            Assert.True(_client.SubscribedStreams.Count() == 1);

            // Subscribe to a different symbol.
            _client.Subscribe(Symbol.LTC_BTC, interval);
            Assert.True(_client.SubscribedStreams.Count() == 2);
        }

        [Fact]
        public void Unsubscribe()
        {
            var symbol = Symbol.BTC_USDT;
            const CandlestickInterval interval = CandlestickInterval.Hour;

            Assert.Empty(_client.SubscribedStreams);

            // Unsubscribe non-subscribed symbol doesn't fail.
            _client.Unsubscribe(symbol, interval);
            Assert.Empty(_client.SubscribedStreams);

            // Subscribe and unsubscribe symbol.
            _client.Subscribe(symbol, interval).Unsubscribe(symbol, interval);

            Assert.Empty(_client.SubscribedStreams);
        }

        [Fact]
        public void UnsubscribeAll()
        {
            const CandlestickInterval interval = CandlestickInterval.Hour;
            var symbols = new string[] { Symbol.BTC_USDT, Symbol.ETH_USDT, Symbol.LTC_USDT };

            // Unsubscribe all when not subscribed doesn't fail.
            _client.Unsubscribe();

            // Subscribe to multiple symbols.
            _client.Subscribe(interval, symbols);
            Assert.True(_client.SubscribedStreams.Count() == symbols.Length);

            // Unsubscribe all.
            _client.Unsubscribe();

            Assert.Empty(_client.SubscribedStreams);
        }
    }
}
