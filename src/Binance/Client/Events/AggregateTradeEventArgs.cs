using System;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    /// <summary>
    /// Aggregate trade client event arguments.
    /// </summary>
    public sealed class AggregateTradeEventArgs : ClientEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the aggregate trade.
        /// </summary>
        public AggregateTrade Trade { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="trade">The aggregate trade.</param>
        public AggregateTradeEventArgs(DateTime time, AggregateTrade trade)
            : base(time)
        {
            Throw.IfNull(trade, nameof(trade));

            Trade = trade;
        }

        #endregion Constructors
    }
}
