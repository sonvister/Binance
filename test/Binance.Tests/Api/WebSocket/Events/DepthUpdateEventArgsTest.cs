using System;
using Xunit;

namespace Binance.Api.WebSocket.Events
{
    public class DepthUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var symbol = Symbol.BTC_USDT;
            long firstUpdateId = 1234567890;
            long lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            Assert.Throws<ArgumentException>("timestamp", () => new DepthUpdateEventArgs(-1, symbol, firstUpdateId, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentException>("timestamp", () => new DepthUpdateEventArgs(0, symbol, firstUpdateId, lastUpdateId, bids, asks));

            Assert.Throws<ArgumentNullException>("symbol", () => new DepthUpdateEventArgs(timestamp, null, firstUpdateId, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentNullException>("symbol", () => new DepthUpdateEventArgs(timestamp, string.Empty, firstUpdateId, lastUpdateId, bids, asks));

            Assert.Throws<ArgumentException>("firstUpdateId", () => new DepthUpdateEventArgs(timestamp, symbol, -1, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(timestamp, symbol, firstUpdateId, -1, bids, asks));
            Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(timestamp, symbol, firstUpdateId, firstUpdateId - 1, bids, asks));

            Assert.Throws<ArgumentNullException>("bids", () => new DepthUpdateEventArgs(timestamp, symbol, firstUpdateId, lastUpdateId, null, asks));
            Assert.Throws<ArgumentNullException>("asks", () => new DepthUpdateEventArgs(timestamp, symbol, firstUpdateId, lastUpdateId, bids, null));
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var symbol = Symbol.BTC_USDT;
            long firstUpdateId = 1234567890;
            long lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var args = new DepthUpdateEventArgs(timestamp, symbol, firstUpdateId, lastUpdateId, bids, asks);

            Assert.Equal(timestamp, args.Timestamp);
            Assert.Equal(symbol, args.Symbol);

            Assert.Equal(firstUpdateId, args.FirstUpdateId);
            Assert.Equal(lastUpdateId, args.LastUpdateId);

            Assert.NotEmpty(args.Bids);
            Assert.NotEmpty(args.Asks);
        }
    }
}
