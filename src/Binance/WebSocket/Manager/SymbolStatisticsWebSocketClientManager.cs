using System;
using Binance.Client;
using Binance.Manager;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// The default <see cref="ISymbolStatisticsWebSocketClientManager"/> implementation.
    /// </summary>
    public class SymbolStatisticsWebSocketClientManager : SymbolStatisticsClientManager<IWebSocketStream>, ISymbolStatisticsWebSocketClientManager
    {
        #region Public Events

        public event EventHandler<ErrorEventArgs> Error
        {
            add => Controller.Error += value;
            remove => Controller.Error -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ISymbolStatisticsClient"/>
        /// and default <see cref="IWebSocketStream"/>, but no logger.
        /// </summary>
        public SymbolStatisticsWebSocketClientManager()
            : this(new SymbolStatisticsClient(), new BinanceWebSocketStreamController())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The web socket stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public SymbolStatisticsWebSocketClientManager(ISymbolStatisticsClient client, IWebSocketStreamController controller, ILogger<SymbolStatisticsWebSocketClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Construtors
    }
}
