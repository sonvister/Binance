using System;
using System.Threading;
using Binance.Client.Events;
using Binance.Market;
using Xunit;

namespace Binance.Tests.WebSocket.Events
{
    public class AggregateTradeEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;

            using (var cts = new CancellationTokenSource())
            {
                Assert.Throws<ArgumentNullException>("trade", () => new AggregateTradeEventArgs(time, cts.Token, null));
            }
        }

        [Fact]
        public void Properties()
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;

            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AggregateTrade(symbol, id, price, quantity, firstTradeId, lastTradeId, time, isBuyerMaker, isBestPriceMatch);

            using (var cts = new CancellationTokenSource())
            {
                var args = new AggregateTradeEventArgs(time, cts.Token, trade);

                Assert.Equal(time, args.Time);
                Assert.Equal(trade, args.Trade);
            }
        }
    }
}
