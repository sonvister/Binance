using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// Account commissions.
    /// </summary>
    public sealed class AccountCommissions
    {
        #region Public Properties

        /// <summary>
        /// Get the maker commission in basis points (bips).
        /// </summary>
        public int Maker { get; }

        /// <summary>
        /// Get the taker commission in basis points (bips).
        /// </summary>
        public int Taker { get; }

        /// <summary>
        /// Get the buyer commission in basis points (bips).
        /// </summary>
        public int Buyer { get; }

        /// <summary>
        /// Get the seller commission in basis points (bips).
        /// </summary>
        public int Seller { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maker">The maker commission (bips).</param>
        /// <param name="taker">The taker commission (bips).</param>
        /// <param name="buyer">The buyer commission (bips).</param>
        /// <param name="seller">The seller commission (bips).</param>
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
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void ThrowIfCommissionIsInvalid(decimal commission, string paramName)
        {
            if (commission < 0 || commission > 10000)
                throw new ArgumentException($"{nameof(AccountCommissions)} commission must be in the range [0-10000] BPS.", paramName);
        }

        #endregion Private Methods
    }
}
