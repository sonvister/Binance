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
        /// Construct an <see cref="IBinanceUser"/> instance providing an API
        /// key and optional API secret. The API secret is not required for 
        /// the user stream methods, but is required for other account methods.
        /// </summary>
        /// <param name="key">The user's API key.</param>
        /// <param name="secret">The user's API secret (optional).</param>
        public BinanceUser(string key, string secret = null)
        {
            Throw.IfNullOrWhiteSpace(key, nameof(key));

            ApiKey = key;

            if (!string.IsNullOrWhiteSpace(secret))
            {
                _hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            }
        }

        #endregion Constructors

        #region Public Methods

        public string Sign(string query)
        {
            if (_hmac == null)
                throw new InvalidOperationException("Signature requires the user's API secret.");

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
