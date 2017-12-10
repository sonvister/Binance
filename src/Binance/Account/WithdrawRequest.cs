using Binance.Api;

namespace Binance.Account
{
    public sealed class WithdrawRequest
    {
        #region Public Properties

        /// <summary>
        /// Get the user.
        /// </summary>
        public IBinanceApiUser User { get; }

        /// <summary>
        /// Get or set the asset.
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// Get or set the address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Get or set the secondary address identifier.
        /// </summary>
        public string AddressTag { get; set; }

        /// <summary>
        /// Get or set the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Get or set a description of the address (optional).
        /// </summary>
        public string Name { get; set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user"></param>
        public WithdrawRequest(IBinanceApiUser user)
        {
            Throw.IfNull(user, nameof(user));

            User = user;
        }

        #endregion Constructors
    }
}
