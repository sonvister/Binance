using Binance.Account;
using Binance.Api;
using Binance.Cache.Events;
using System;
using Xunit;

namespace Binance.Tests.Cache.Events
{
    public class AccountInfoCacheEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentNullException>("accountInfo", () => new AccountInfoCacheEventArgs(null));
        }

        [Fact]
        public void Properties()
        {
            var user = new BinanceApiUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            long updateTime = 1234567890;
            var balances = new[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var accountInfo = new AccountInfo(user, commissions, status, updateTime, balances);

            var args = new AccountInfoCacheEventArgs(accountInfo);

            Assert.Equal(accountInfo, args.AccountInfo);
        }
    }
}
