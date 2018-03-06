using System;
using System.Linq;
using Binance.Client;
using Xunit;

namespace Binance.Tests.Client
{
    public class AggregateTradeClientTests
    {
        private IAggregateTradeClient _client;

        public AggregateTradeClientTests()
        {
            _client = new AggregateTradeClient();
        }

        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentNullException>("symbol", () => _client.Subscribe((string)null));
            Assert.Throws<ArgumentNullException>("symbol", () => _client.Subscribe(string.Empty));

            Assert.Throws<ArgumentNullException>("symbol", () => _client.Unsubscribe((string)null));
            Assert.Throws<ArgumentNullException>("symbol", () => _client.Unsubscribe(string.Empty));
        }

        [Fact]
        public void Subscribe()
        {
            var symbol = Symbol.BTC_USDT;

            Assert.Empty(_client.SubscribedStreams);

            // Subscribe to symbol.
            _client.Subscribe(symbol);
            Assert.True(_client.SubscribedStreams.Count() == 1);

            // Re-Subscribe to same symbol doesn't fail.
            _client.Subscribe(symbol);
            Assert.True(_client.SubscribedStreams.Count() == 1);

            // Subscribe to a different symbol.
            _client.Subscribe(Symbol.LTC_BTC);
            Assert.True(_client.SubscribedStreams.Count() == 2);
        }

        [Fact]
        public void Unsubscribe()
        {
            var symbol = Symbol.BTC_USDT;

            Assert.Empty(_client.SubscribedStreams);

            // Unsubscribe non-subscribed symbol doesn't fail.
            _client.Unsubscribe(symbol);
            Assert.Empty(_client.SubscribedStreams);

            // Subscribe and unsubscribe symbol.
            _client.Subscribe(symbol).Unsubscribe(symbol);

            Assert.Empty(_client.SubscribedStreams);
        }

        [Fact]
        public void UnsubscribeAll()
        {
            var symbols = new string[] { Symbol.BTC_USDT, Symbol.ETH_USDT, Symbol.LTC_USDT };

            // Unsubscribe all when not subscribed doesn't fail.
            _client.Unsubscribe();

            // Subscribe to multiple symbols.
            _client.Subscribe(symbols);
            Assert.True(_client.SubscribedStreams.Count() == symbols.Length);

            // Unsubscribe all.
            _client.Unsubscribe();

            Assert.Empty(_client.SubscribedStreams);
        }
    }
}
