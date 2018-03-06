// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// Account status.
    /// </summary>
    public sealed class AccountStatus
    {
        #region Public Properties

        /// <summary>
        /// Get the flag indicating if the account can trade.
        /// </summary>
        public bool CanTrade { get; }

        /// <summary>
        /// Get the flag indicating if the account can withdraw.
        /// </summary>
        public bool CanWithdraw { get; }

        /// <summary>
        /// Get the flag indicating if the account can deposit.
        /// </summary>
        public bool CanDeposit { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="canTrade">Can trade status.</param>
        /// <param name="canWithdraw">Can withdraw status.</param>
        /// <param name="canDeposit">Can deposit status.</param>
        public AccountStatus(bool canTrade, bool canWithdraw, bool canDeposit)
        {
            CanTrade = canTrade;
            CanWithdraw = canWithdraw;
            CanDeposit = canDeposit;
        }

        #endregion Constructors
    }
}
