using System.Linq;
using Binance.Client;
using Xunit;

namespace Binance.Tests.Client
{
    public class SymbolStatisticsClientTests
    {
        private ISymbolStatisticsClient _client;

        public SymbolStatisticsClientTests()
        {
            _client = new SymbolStatisticsClient();
        }

        [Fact]
        public void Subscribe()
        {
            var symbol = Symbol.BTC_USDT;

            Assert.Empty(_client.ObservedStreams);

            // Subscribe to symbol.
            _client.Subscribe(symbol);
            Assert.True(_client.ObservedStreams.Count() == 1);

            // Re-Subscribe to same symbol doesn't fail.
            _client.Subscribe(symbol);
            Assert.True(_client.ObservedStreams.Count() == 1);

            // Subscribe to a different symbol.
            _client.Subscribe(Symbol.LTC_BTC);
            Assert.True(_client.ObservedStreams.Count() == 2);
        }

        [Fact]
        public void Unsubscribe()
        {
            var symbol = Symbol.BTC_USDT;

            Assert.Empty(_client.ObservedStreams);

            // Unsubscribe non-subscribed symbol doesn't fail.
            _client.Unsubscribe(symbol);
            Assert.Empty(_client.ObservedStreams);

            // Subscribe and unsubscribe symbol.
            _client.Subscribe(symbol).Unsubscribe(symbol);

            Assert.Empty(_client.ObservedStreams);
        }

        [Fact]
        public void UnsubscribeAll()
        {
            var symbols = new string[] { Symbol.BTC_USDT, Symbol.ETH_USDT, Symbol.LTC_USDT };

            // Unsubscribe all when not subscribed doesn't fail.
            _client.Unsubscribe();

            // Subscribe to multiple symbols.
            _client.Subscribe(symbols);
            Assert.True(_client.ObservedStreams.Count() == symbols.Length);

            // Unsubscribe all.
            _client.Unsubscribe();

            Assert.Empty(_client.ObservedStreams);
        }
    }
}
