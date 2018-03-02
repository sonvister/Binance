using System;
using Binance.Client;
using Binance.Manager;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// The default <see cref="IDepthWebSocketClientManager"/> implementation.
    /// </summary>
    public class DepthWebSocketClientManager : DepthClientManager<IWebSocketStream>, IDepthWebSocketClientManager
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
        /// Default constructor provides default <see cref="IDepthClient"/>
        /// and default <see cref="IWebSocketStream"/>, but no logger.
        /// </summary>
        public DepthWebSocketClientManager()
            : this(new DepthClient(), new BinanceWebSocketStreamController())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The web socket stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public DepthWebSocketClientManager(IDepthClient client, IWebSocketStreamController controller, ILogger<DepthWebSocketClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Construtors
    }
}
