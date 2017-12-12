using System;
using System.Threading;
using Binance.Account;
using Binance.Api;
using Binance.Api.WebSocket.Events;
using Xunit;

namespace Binance.Tests.Api.WebSocket.Events
{
    public class AccountUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var user = new BinanceApiUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            long updateTime = 1234567890;
            var balances = new[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new AccountInfo(user, commissions, status, updateTime, balances);

            using (var cts = new CancellationTokenSource())
            {
                Assert.Throws<ArgumentException>("timestamp", () => new AccountUpdateEventArgs(-1, cts.Token, account));
                Assert.Throws<ArgumentException>("timestamp", () => new AccountUpdateEventArgs(0, cts.Token, account));
                Assert.Throws<ArgumentNullException>("accountInfo", () => new AccountUpdateEventArgs(timestamp, cts.Token, null));
            }
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var user = new BinanceApiUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            long updateTime = 1234567890;
            var balances = new[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new AccountInfo(user, commissions, status, updateTime, balances);

            using (var cts = new CancellationTokenSource())
            {
                var args = new AccountUpdateEventArgs(timestamp, cts.Token, account);

                Assert.Equal(timestamp, args.Timestamp);
                Assert.Equal(account, args.AccountInfo);
            }
        }
    }
}
