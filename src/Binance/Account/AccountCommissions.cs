using System;

namespace Binance.Account
{
    /// <summary>
    /// Account commissions.
    /// </summary>
    public sealed class AccountCommissions
    {
        #region Public Properties

        /// <summary>
        /// Get the maker commission.
        /// </summary>
        public int Maker { get; }

        /// <summary>
        /// Get the taker commission.
        /// </summary>
        public int Taker { get; }

        /// <summary>
        /// Get the buyer commission.
        /// </summary>
        public int Buyer { get; }

        /// <summary>
        /// Get the seller commission.
        /// </summary>
        public int Seller { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maker">The maker commission.</param>
        /// <param name="taker">The taker commission.</param>
        /// <param name="buyer">The buyer commission.</param>
        /// <param name="seller">The seller commission.</param>
        public AccountCommissions(int maker, int taker, int buyer, int seller)
        {
            ThrowIfCommissionIsInvalid(maker, nameof(maker));
            ThrowIfCommissionIsInvalid(taker, nameof(taker));
            ThrowIfCommissionIsInvalid(buyer, nameof(buyer));
            ThrowIfCommissionIsInvalid(seller, nameof(seller));

            Maker = maker;
            Taker = taker;
            Buyer = buyer;
            Seller = seller;
        }

        #endregion Constructors

        #region Private Methods

        /// <summary>
        /// Verify commission argument is valid.
        /// </summary>
        /// <param name="commission">The commission.</param>
        /// <param name="paramName">The parameter name.</param>
        internal static void ThrowIfCommissionIsInvalid(decimal commission, string paramName)
        {
            if (commission < 0 || commission > 100)
                throw new ArgumentException($"{nameof(AccountCommissions)} commission must be in the range [0-100].", paramName);
        }

        #endregion Private Methods
    }
}
