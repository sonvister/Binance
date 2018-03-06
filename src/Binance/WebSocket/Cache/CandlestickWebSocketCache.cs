using System;
using Binance.Cache;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ICandlestickWebSocketCache"/> implementation.
    /// </summary>
    public class CandlestickWebSocketCache : CandlestickCache<ICandlestickWebSocketClient>, ICandlestickWebSocketCache
    {
        #region Public Events

        public event EventHandler<ErrorEventArgs> Error
        {
            add => Client.Error += value;
            remove => Client.Error -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IBinanceApi"/>
        /// and default <see cref="ICandlestickWebSocketClient"/>, but no logger.
        /// </summary>
        public CandlestickWebSocketCache()
            : this(new BinanceApi(), new CandlestickWebSocketClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance API (required).</param>
        /// <param name="client">The web socket client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickWebSocketCache(IBinanceApi api, ICandlestickWebSocketClient client, ILogger<CandlestickWebSocketCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Construtors
    }
}
