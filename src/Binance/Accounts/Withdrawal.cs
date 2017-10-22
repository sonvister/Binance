namespace Binance.Accounts
{
    public sealed class Withdrawal : IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the asset.
        /// </summary>
        public string Asset { get; private set; }

        /// <summary>
        /// Get the amount.
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Get the apply time.
        /// </summary>
        public long Timestamp { get; private set; }

        /// <summary>
        /// Get the status.
        /// </summary>
        public WithdrawalStatus Status { get; private set; }

        /// <summary>
        /// Get the address.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Get the transaction ID.
        /// </summary>
        public string TxId { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="status">The status.</param>
        /// <param name="address">The address.</param>
        /// <param name="txId">The transaction ID.</param>
        public Withdrawal(string asset, decimal amount, long timestamp, WithdrawalStatus status, string address, string txId = null)
        {
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));
            Throw.IfNullOrWhiteSpace(address, nameof(address));

            Asset = asset;
            Amount = amount;
            Timestamp = timestamp;
            Status = status;
            Address = address;

            TxId = txId;
        }

        #endregion Constructors
    }
}
