using System;
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

            Assert.Throws<ArgumentNullException>("accountInfo", () => new AccountUpdateEventArgs(time, null));
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

            var args = new AccountUpdateEventArgs(time, account);

            Assert.Equal(time, args.Time);
            Assert.Equal(account, args.AccountInfo);
        }
    }
}
