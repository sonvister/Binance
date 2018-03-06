// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class DepositAddress
    {
        #region Public Properties

        /// <summary>
        /// Get the asset symbol.
        /// </summary>
        public string Asset { get; }

        /// <summary>
        /// Get the address.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Get the address tag.
        /// </summary>
        public string AddressTag { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="address"></param>
        /// <param name="addressTag"></param>
        public DepositAddress(string asset, string address, string addressTag = null)
        {
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));
            Throw.IfNullOrWhiteSpace(address, nameof(address));

            Asset = asset.FormatSymbol();
            Address = address;
            AddressTag = addressTag;
        }

        #endregion Constructors
    }
}
