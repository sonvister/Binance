using System;
using System.Security.Cryptography;
using System.Text;

namespace Binance.Api
{
    /// <summary>
    /// Binance user <see cref="IBinanceUser"/> implementation.
    /// </summary>
    public sealed class BinanceUser : IBinanceUser
    {
        #region Public Properties

        public string ApiKey { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private HMAC _hmac;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">The user's API key.</param>
        /// <param name="secret">The user's API secret.</param>
        public BinanceUser(string key, string secret)
        {
            Throw.IfNullOrWhiteSpace(key, nameof(key));
            Throw.IfNullOrWhiteSpace(secret, nameof(secret));

            ApiKey = key;

            _hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        }

        #endregion Constructors

        #region Public Methods

        public string Sign(string query)
        {
            var hash = _hmac.ComputeHash(Encoding.UTF8.GetBytes(query));

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        #endregion Public Methods

        #region IDisposable

        private bool _disposed = false;

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _hmac?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
