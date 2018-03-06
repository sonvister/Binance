using System;
using Binance.Cache;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IDepthWebSocketCache"/> implementation.
    /// </summary>
    public class DepthWebSocketCache : OrderBookCache<IDepthWebSocketClient>, IDepthWebSocketCache
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
        /// and default <see cref="IDepthWebSocketClient"/>, but no logger.
        /// </summary>
        public DepthWebSocketCache()
            : this(new BinanceApi(), new DepthWebSocketClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance API (required).</param>
        /// <param name="client">The web socket client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public DepthWebSocketCache(IBinanceApi api, IDepthWebSocketClient client, ILogger<DepthWebSocketCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Construtors
    }
}
