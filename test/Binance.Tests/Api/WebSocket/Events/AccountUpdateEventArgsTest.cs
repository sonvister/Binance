using Binance.Accounts;
using Binance.Api.WebSocket.Events;
using System;
using Xunit;

namespace Binance.Tests.Api.WebSocket.Events
{
    public class AccountUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            var balances = new AccountBalance[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new Account(commissions, status);

            Assert.Throws<ArgumentException>("timestamp", () => new AccountUpdateEventArgs(-1, account));
            Assert.Throws<ArgumentException>("timestamp", () => new AccountUpdateEventArgs(0, account));
            Assert.Throws<ArgumentNullException>("account", () => new AccountUpdateEventArgs(timestamp, null));
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            var balances = new AccountBalance[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new Account(commissions, status);

            var args = new AccountUpdateEventArgs(timestamp, account);

            Assert.Equal(timestamp, args.Timestamp);
            Assert.Equal(account, args.Account);
        }
    }
}
