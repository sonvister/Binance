using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class Withdrawal : IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Get the asset.
        /// </summary>
        public string Asset { get; }

        /// <summary>
        /// Get the amount.
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Get the apply time.
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// Get the status.
        /// </summary>
        public WithdrawalStatus Status { get; }

        /// <summary>
        /// Get the address.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Get the address tag.
        /// </summary>
        public string AddressTag { get; }

        /// <summary>
        /// Get the transaction ID.
        /// </summary>
        public string TxId { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="asset">The asset.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="time">The time.</param>
        /// <param name="status">The status.</param>
        /// <param name="address">The address.</param>
        /// <param name="addressTag">The address tag.</param>
        /// <param name="txId">The transaction ID.</param>
        public Withdrawal(string id, string asset, decimal amount, DateTime time, WithdrawalStatus status, string address, string addressTag = null, string txId = null)
        {
            Throw.IfNullOrWhiteSpace(id, nameof(id));
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));
            Throw.IfNullOrWhiteSpace(address, nameof(address));

            if (amount <= 0)
                throw new ArgumentException($"{nameof(Withdrawal)}: amount must be greater than 0.", nameof(amount));

            Id = id;
            Asset = asset.FormatSymbol();
            Amount = amount;
            Time = time;
            Status = status;
            Address = address;
            AddressTag = addressTag;
            TxId = txId;
        }

        #endregion Constructors
    }
}
