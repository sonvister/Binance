using Binance.Account;
using System;
using Xunit;

namespace Binance.Api.WebSocket.Events.Tests
{
    public class AccountUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var user = new BinanceUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            var balances = new AccountBalance[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new AccountInfo(user, commissions, status, balances);

            Assert.Throws<ArgumentException>("timestamp", () => new AccountUpdateEventArgs(-1, account));
            Assert.Throws<ArgumentException>("timestamp", () => new AccountUpdateEventArgs(0, account));
            Assert.Throws<ArgumentNullException>("account", () => new AccountUpdateEventArgs(timestamp, null));
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var user = new BinanceUser("api-key");
            var commissions = new AccountCommissions(10, 10, 0, 0);
            var status = new AccountStatus(true, true, true);
            var balances = new AccountBalance[] { new AccountBalance("BTC", 0.1m, 0.2m) };

            var account = new AccountInfo(user, commissions, status, balances);

            var args = new AccountUpdateEventArgs(timestamp, account);

            Assert.Equal(timestamp, args.Timestamp);
            Assert.Equal(account, args.Account);
        }
    }
}
