using System;
using Binance.Api;
using Binance.Cache;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ITradeWebSocketCache"/> implementation.
    /// </summary>
    public class TradeWebSocketCache : TradeCache<ITradeWebSocketClient>, ITradeWebSocketCache
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
        /// and default <see cref="ITradeWebSocketClient"/>, but no logger.
        /// </summary>
        public TradeWebSocketCache()
            : this(new BinanceApi(), new TradeWebSocketClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance API (required).</param>
        /// <param name="client">The web socket client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public TradeWebSocketCache(IBinanceApi api, ITradeWebSocketClient client, ILogger<TradeWebSocketCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Construtors
    }
}
