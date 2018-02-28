using System;
using Binance.Client;
using Binance.Manager;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// The default <see cref="ICandlestickWebSocketClientManager"/> implementation.
    /// </summary>
    public class CandlestickWebSocketClientManager : CandlestickClientManager<IWebSocketStream>, ICandlestickWebSocketClientManager
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
        /// Default constructor provides default <see cref="ICandlestickClient"/>
        /// and default <see cref="IWebSocketStream"/>, but no logger.
        /// </summary>
        public CandlestickWebSocketClientManager()
            : this(new CandlestickClient(), new BinanceWebSocketStreamController())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The web socket stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickWebSocketClientManager(ICandlestickClient client, IWebSocketStreamController controller, ILogger<CandlestickWebSocketClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Construtors
    }
}
