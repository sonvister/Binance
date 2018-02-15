namespace Binance.Client.Events
{
    public enum OrderExecutionType
    {
        /// <summary>
        /// New.
        /// </summary>
        New,

        /// <summary>
        /// Cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// Replaced.
        /// </summary>
        Replaced,
        
        /// <summary>
        /// Rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Trade.
        /// </summary>
        Trade,

        /// <summary>
        /// Expired.
        /// </summary>
        Expired
    }
}
