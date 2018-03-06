using System;
using System.Linq;
using Xunit;

namespace Binance.Tests.Account
{
    public class AccountInfoTest
    {
        [Fact]
        public void Throws()
        {
            var user = new BinanceApiUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            var status = new AccountStatus(true, true, true);

            Assert.Throws<ArgumentNullException>("user", () => new AccountInfo(null, commissions, status, time));
            Assert.Throws<ArgumentNullException>("commissions", () => new AccountInfo(user, null, status, time));
            Assert.Throws<ArgumentNullException>("status", () => new AccountInfo(user, commissions, null, time));
        }

        [Fact]
        public void Properties()
        {
            var user = new BinanceApiUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            var balances = new[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new AccountInfo(user, commissions, status, time);

            Assert.Equal(commissions, account.Commissions);
            Assert.Equal(status, account.Status);
            Assert.Equal(time, account.Time);
            Assert.NotNull(account.Balances);
            Assert.Empty(account.Balances);

            account = new AccountInfo(user, commissions, status, time, balances);

            Assert.Equal(commissions, account.Commissions);
            Assert.Equal(status, account.Status);
            Assert.Equal(time, account.Time);
            Assert.NotNull(account.Balances);
            Assert.NotEmpty(account.Balances);
            Assert.Equal(balances[0].Asset, account.Balances.First().Asset);
        }
    }
}
