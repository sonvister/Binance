using System;
using Binance.Api;
using Binance.Cache;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ISymbolStatisticsWebSocketCache"/> implementation.
    /// </summary>
    public class SymbolStatisticsWebSocketCache : SymbolStatisticsCache<ISymbolStatisticsWebSocketClient>, ISymbolStatisticsWebSocketCache
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
        /// and default <see cref="ISymbolStatisticsWebSocketClient"/>, but no logger.
        /// </summary>
        public SymbolStatisticsWebSocketCache()
            : this(new BinanceApi(), new SymbolStatisticsWebSocketClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance API (required).</param>
        /// <param name="client">The web socket client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public SymbolStatisticsWebSocketCache(IBinanceApi api, ISymbolStatisticsWebSocketClient client, ILogger<SymbolStatisticsWebSocketCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Construtors
    }
}
