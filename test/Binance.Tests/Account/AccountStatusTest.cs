using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Binance.Tests.Account
{
    public class AccountStatusTest
    {
        [Fact]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public void Properties()
        {
            var canTrade = true;
            var canWithdraw = false;
            var canDeposit = false;

            var status = new AccountStatus(canTrade, canWithdraw, canDeposit);

            Assert.Equal(canTrade, status.CanTrade);
            Assert.Equal(canWithdraw, status.CanWithdraw);
            Assert.Equal(canDeposit, status.CanDeposit);

            canTrade = false;
            canWithdraw = true;

            status = new AccountStatus(canTrade, canWithdraw, canDeposit);

            Assert.Equal(canTrade, status.CanTrade);
            Assert.Equal(canWithdraw, status.CanWithdraw);
            Assert.Equal(canDeposit, status.CanDeposit);

            canWithdraw = false;
            canDeposit = true;

            status = new AccountStatus(canTrade, canWithdraw, canDeposit);

            Assert.Equal(canTrade, status.CanTrade);
            Assert.Equal(canWithdraw, status.CanWithdraw);
            Assert.Equal(canDeposit, status.CanDeposit);
        }
    }
}
