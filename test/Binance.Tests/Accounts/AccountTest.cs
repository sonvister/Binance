using Binance.Accounts;
using Binance.Api;
using System;
using System.Linq;
using Xunit;

namespace Binance.Tests.Accounts
{
    public class AccountTest
    {
        [Fact]
        public void Throws()
        {
            var user = new BinanceUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);

            var account = new Account(user, commissions, status);

            Assert.Throws<ArgumentNullException>("user", () => new Account(null, commissions, status));
            Assert.Throws<ArgumentNullException>("commissions", () => new Account(user, null, status));
            Assert.Throws<ArgumentNullException>("status", () => new Account(user, commissions, null));
        }

        [Fact]
        public void Properties()
        {
            var user = new BinanceUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            var balances = new AccountBalance[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new Account(user, commissions, status);

            Assert.Equal(commissions, account.Commissions);
            Assert.Equal(status, account.Status);
            Assert.NotNull(account.Balances);
            Assert.Empty(account.Balances);

            account = new Account(user, commissions, status, balances);

            Assert.Equal(commissions, account.Commissions);
            Assert.Equal(status, account.Status);
            Assert.NotNull(account.Balances);
            Assert.NotEmpty(account.Balances);
            Assert.Equal(balances[0].Asset, account.Balances.First().Asset);
        }
    }
}
