using System;
using Xunit;

namespace Binance.Api.WebSocket.Events
{
    public class DepthWebSocketEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var symbol = "BTCUSDT";
            var firstUpdateId = 1234567890;
            var lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            Assert.Throws<ArgumentException>("timestamp", () => new DepthUpdateEventArgs(-1, null, firstUpdateId, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentException>("timestamp", () => new DepthUpdateEventArgs(0, null, firstUpdateId, lastUpdateId, bids, asks));

            Assert.Throws<ArgumentNullException>("symbol", () => new DepthUpdateEventArgs(time, null, firstUpdateId, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentNullException>("symbol", () => new DepthUpdateEventArgs(time, "", firstUpdateId, lastUpdateId, bids, asks));

            Assert.Throws<ArgumentException>("firstUpdateId", () => new DepthUpdateEventArgs(time, symbol, -1, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentException>("firstUpdateId", () => new DepthUpdateEventArgs(time, symbol, 0, lastUpdateId, bids, asks));

            Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, -1, bids, asks));
            Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, 0, bids, asks));
            Assert.Throws<ArgumentException>("lastUpdateId", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, firstUpdateId - 1, bids, asks));

            Assert.Throws<ArgumentNullException>("bids", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, lastUpdateId, null, asks));
            Assert.Throws<ArgumentNullException>("asks", () => new DepthUpdateEventArgs(time, symbol, firstUpdateId, lastUpdateId, bids, null));
        }

        [Fact]
        public void Properties()
        {
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var symbol = "BTCUSDT";
            var firstUpdateId = 1234567890;
            var lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var args = new DepthUpdateEventArgs(time, symbol, firstUpdateId, lastUpdateId, bids, asks);

            Assert.Equal(time, args.Timestamp);
            Assert.Equal(symbol, args.Symbol);

            Assert.Equal(firstUpdateId, args.FirstUpdateId);
            Assert.Equal(lastUpdateId, args.LastUpdateId);

            Assert.NotEmpty(args.Bids);
            Assert.NotEmpty(args.Asks);
        }
    }
}
