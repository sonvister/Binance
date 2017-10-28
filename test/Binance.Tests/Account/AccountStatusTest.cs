using Xunit;

namespace Binance.Account.Tests
{
    public class AccountStatusTest
    {
        [Fact]
        public void Properties()
        {
            bool canTrade = true;
            bool canWithdraw = false;
            bool canDeposit = false;

            var status = new AccountStatus(canTrade, canWithdraw, canDeposit);

            Assert.Equal(canTrade, status.CanTrade);
            Assert.Equal(canWithdraw, status.CanWithdraw);
            Assert.Equal(canDeposit, status.CanDeposit);

            canTrade = false;
            canWithdraw = true;
            canDeposit = false;

            status = new AccountStatus(canTrade, canWithdraw, canDeposit);

            Assert.Equal(canTrade, status.CanTrade);
            Assert.Equal(canWithdraw, status.CanWithdraw);
            Assert.Equal(canDeposit, status.CanDeposit);

            canTrade = false;
            canWithdraw = false;
            canDeposit = true;

            status = new AccountStatus(canTrade, canWithdraw, canDeposit);

            Assert.Equal(canTrade, status.CanTrade);
            Assert.Equal(canWithdraw, status.CanWithdraw);
            Assert.Equal(canDeposit, status.CanDeposit);
        }
    }
}
