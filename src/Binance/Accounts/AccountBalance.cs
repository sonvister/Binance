namespace Binance.Accounts
{
    /// <summary>
    /// An account asset balance.
    /// </summary>
    public class AccountBalance
    {
        #region Public Properties

        /// <summary>
        /// Get the asset.
        /// </summary>
        public string Asset { get; private set; }

        /// <summary>
        /// Get the free (available) amount.
        /// </summary>
        public decimal Free { get; private set; }

        /// <summary>
        /// Get the locked (on hold) amount.
        /// </summary>
        public decimal Locked { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="free">The free amount.</param>
        /// <param name="locked">The locked amount.</param>
        public AccountBalance(string asset, decimal free, decimal locked)
        {
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));

            Asset = asset;
            Free = free;
            Locked = locked;
        }

        #endregion Constructors
    }
}
