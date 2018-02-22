using System;
using System.Threading;
using Binance.Account;
using Binance.Api;
using Binance.Client.Events;
using Xunit;

namespace Binance.Tests.Client.Events
{
    public class AccountUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;

            using (var cts = new CancellationTokenSource())
            {
                Assert.Throws<ArgumentNullException>("accountInfo", () => new AccountUpdateEventArgs(time, cts.Token, null));
            }
        }

        [Fact]
        public void Properties()
        {
            var user = new BinanceApiUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            var balances = new[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new AccountInfo(user, commissions, status, time, balances);

            using (var cts = new CancellationTokenSource())
            {
                var args = new AccountUpdateEventArgs(time, cts.Token, account);

                Assert.Equal(time, args.Time);
                Assert.Equal(account, args.AccountInfo);
            }
        }
    }
}
