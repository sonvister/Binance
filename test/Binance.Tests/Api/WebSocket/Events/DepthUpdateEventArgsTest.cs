using System;
using System.Threading;
using Binance.Api.WebSocket.Events;
using Xunit;

namespace Binance.Tests.Api.WebSocket.Events
{
    public class DepthUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var symbol = Symbol.BTC_USDT;
            const long firstUpdateId = 1234567890;
            const long lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            using (var cts = new CancellationTokenSource())
            {
                Assert.Throws<ArgumentException>("timestamp", () => new DepthUpdateEventArgs(-1, cts.Token, symbol, firstUpdateId, lastUpdateId, bids, asks));
                Assert.Throws<ArgumentException>("timestamp", () => new DepthUpdateEventArgs(0, cts.Token, symbol, firstUpdateId, lastUpdateId, bids, asks));

                Assert.Throws<ArgumentNullException>("symbol", () => new DepthUpdateEventArgs(timestamp, cts.Token, null, firstUpdateId, lastUpdateId, bids, asks));
                Assert.Throws<ArgumentNullException>("symbol", () => new DepthUpdateEventArgs(timestamp, cts.Token, string.Empty, firstUpdateId, lastUpdateId, bids, asks));

                Assert.Throws<ArgumentException>("firstUpdateId", () => new DepthUpdateEventArgs(timestamp, cts.Token, symbol, -1, lastUpdateId, bids, asks));
                Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(timestamp, cts.Token, symbol, firstUpdateId, -1, bids, asks));
                Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(timestamp, cts.Token, symbol, firstUpdateId, firstUpdateId - 1, bids, asks));

                Assert.Throws<ArgumentNullException>("bids", () => new DepthUpdateEventArgs(timestamp, cts.Token, symbol, firstUpdateId, lastUpdateId, null, asks));
                Assert.Throws<ArgumentNullException>("asks", () => new DepthUpdateEventArgs(timestamp, cts.Token, symbol, firstUpdateId, lastUpdateId, bids, null));
            }
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var symbol = Symbol.BTC_USDT;
            const long firstUpdateId = 1234567890;
            const long lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            using (var cts = new CancellationTokenSource())
            {
                var args = new DepthUpdateEventArgs(timestamp, cts.Token, symbol, firstUpdateId, lastUpdateId, bids, asks);

                Assert.Equal(timestamp, args.Timestamp);
                Assert.Equal(symbol, args.Symbol);

                Assert.Equal(firstUpdateId, args.FirstUpdateId);
                Assert.Equal(lastUpdateId, args.LastUpdateId);

                Assert.NotEmpty(args.Bids);
                Assert.NotEmpty(args.Asks);
            }
        }
    }
}
