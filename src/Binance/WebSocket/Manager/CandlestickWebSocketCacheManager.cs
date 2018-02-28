using System;
using Binance.Api;
using Binance.Cache;
using Binance.Manager;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// The default <see cref="ICandlestickWebSocketCacheManager"/> implementation.
    /// </summary>
    public class CandlestickWebSocketCacheManager : CandlestickCache, ICandlestickWebSocketCacheManager
    {
        #region Public Events

        public event EventHandler<ErrorEventArgs> Error
        {
            add => _manager.Controller.Error += value;
            remove => _manager.Controller.Error -= value;
        }

        #endregion Public Events

        #region Public Properties

        public IJsonStreamController<IWebSocketStream> Controller => _manager.Controller;

        public IWatchdogTimer Watchdog => _manager.Watchdog;

        #endregion Public Properties

        #region Private Fields

        private readonly ICandlestickWebSocketClientManager _manager;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IBinanceApi"/>
        /// and default <see cref="ICandlestickWebSocketClientManager"/>, but no logger.
        /// </summary>
        public CandlestickWebSocketCacheManager()
            : this(new BinanceApi(), new CandlestickWebSocketClientManager())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance API (required).</param>
        /// <param name="manager">The web socket client manager (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickWebSocketCacheManager(IBinanceApi api, ICandlestickWebSocketClientManager manager, ILogger<CandlestickWebSocketCacheManager> logger = null)
            : base(api, manager, logger) // use manager as client.
        {
            Throw.IfNull(manager, nameof(manager));

            _manager = manager;
        }

        #endregion Construtors

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _manager?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
