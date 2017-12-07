namespace Binance
{
    public enum SymbolStatus
    {
        /// <summary>
        /// Pre-trading.
        /// </summary>
        PreTrading,

        /// <summary>
        /// Trading.
        /// </summary>
        Trading,

        /// <summary>
        /// Post-trading
        /// </summary>
        PostTrading,

        /// <summary>
        /// End-of-day
        /// </summary>
        EndOfDay,

        /// <summary>
        /// Halt.
        /// </summary>
        Halt,

        /// <summary>
        /// Auction match.
        /// </summary>
        AuctionMatch,

        /// <summary>
        /// Break.
        /// </summary>
        Break
    }
}
