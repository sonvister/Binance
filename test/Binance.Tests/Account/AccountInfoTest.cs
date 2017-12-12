using System;
using System.Linq;
using Binance.Account;
using Binance.Api;
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
            long updateTime = 1234567890;
            var status = new AccountStatus(true, true, true);

            Assert.Throws<ArgumentNullException>("user", () => new AccountInfo(null, commissions, status, updateTime));
            Assert.Throws<ArgumentNullException>("commissions", () => new AccountInfo(user, null, status, updateTime));
            Assert.Throws<ArgumentNullException>("status", () => new AccountInfo(user, commissions, null, updateTime));
            Assert.Throws<ArgumentException>("updateTime", () => new AccountInfo(user, commissions, status, -1));
            Assert.Throws<ArgumentException>("updateTime", () => new AccountInfo(user, commissions, status, 0));
        }

        [Fact]
        public void Properties()
        {
            var user = new BinanceApiUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            long updateTime = 1234567890;
            var balances = new[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new AccountInfo(user, commissions, status, updateTime);

            Assert.Equal(commissions, account.Commissions);
            Assert.Equal(status, account.Status);
            Assert.Equal(updateTime, account.Timestamp);
            Assert.NotNull(account.Balances);
            Assert.Empty(account.Balances);

            account = new AccountInfo(user, commissions, status, updateTime, balances);

            Assert.Equal(commissions, account.Commissions);
            Assert.Equal(status, account.Status);
            Assert.Equal(updateTime, account.Timestamp);
            Assert.NotNull(account.Balances);
            Assert.NotEmpty(account.Balances);
            Assert.Equal(balances[0].Asset, account.Balances.First().Asset);
        }
    }
}
