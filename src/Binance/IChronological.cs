namespace Binance
{
    public interface IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the timestamp in milliseconds (Unix time).
        /// </summary>
        long Timestamp { get; }

        #endregion Public Properties
    }
}
