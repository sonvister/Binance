using System;

namespace Binance
{
    public interface IChronological
    {
        #region Properties

        /// <summary>
        /// Get the time (UTC).
        /// </summary>
        DateTime Time { get; }

        #endregion Properties
    }
}
