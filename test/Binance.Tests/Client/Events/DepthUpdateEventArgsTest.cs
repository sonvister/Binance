using System;
using Binance.Client.Events;
using Xunit;

namespace Binance.Tests.Client.Events
{
    public class DepthUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            var symbol = Symbol.BTC_USDT;
            const long firstUpdateId = 1234567890;
            const long lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            Assert.Throws<ArgumentNullException>("symbol", () => new DepthUpdateEventArgs(time, null, firstUpdateId, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentNullException>("symbol", () => new DepthUpdateEventArgs(time, string.Empty, firstUpdateId, lastUpdateId, bids, asks));

            Assert.Throws<ArgumentException>("firstUpdateId", () => new DepthUpdateEventArgs(time, symbol, -1, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, -1, bids, asks));
            Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, firstUpdateId - 1, bids, asks));

            Assert.Throws<ArgumentNullException>("bids", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, lastUpdateId, null, asks));
            Assert.Throws<ArgumentNullException>("asks", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, lastUpdateId, bids, null));
        }

        [Fact]
        public void Properties()
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            var symbol = Symbol.BTC_USDT;
            const long firstUpdateId = 1234567890;
            const long lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var args = new DepthUpdateEventArgs(time, symbol, firstUpdateId, lastUpdateId, bids, asks);

            Assert.Equal(time, args.Time);
            Assert.Equal(symbol, args.Symbol);

            Assert.Equal(firstUpdateId, args.FirstUpdateId);
            Assert.Equal(lastUpdateId, args.LastUpdateId);

            Assert.NotEmpty(args.Bids);
            Assert.NotEmpty(args.Asks);
        }
    }
}
