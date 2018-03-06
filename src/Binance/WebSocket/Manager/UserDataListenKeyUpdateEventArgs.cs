using System;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public sealed class UserDataListenKeyUpdateEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the user.
        /// </summary>
        public IBinanceApiUser User { get; }

        /// <summary>
        /// Get the old listen key.
        /// </summary>
        public string OldListenKey { get; }

        /// <summary>
        /// Get the new listen key.
        /// </summary>
        public string NewListenKey { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldListenKey"></param>
        /// <param name="newListenKey"></param>
        public UserDataListenKeyUpdateEventArgs(IBinanceApiUser user, string oldListenKey, string newListenKey)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(oldListenKey, nameof(oldListenKey));

            User = user;
            OldListenKey = oldListenKey;
            NewListenKey = newListenKey; // can be null.
        }

        #endregion Constructors
    }
}
