using Binance.Client;
using Binance.Manager;
using Binance.Stream;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// The default <see cref="ICandlestickWebSocketClientManager"/> implementation.
    /// </summary>
    public class CandlestickWebSocketClientManager : CandlestickClientManager<IWebSocketStream>, ICandlestickWebSocketClientManager
    {
        #region Public Properties

        IJsonStreamController<IJsonStream> IControllerManager<IJsonStream>.Controller => Controller;

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ICandlestickClient"/>
        /// and default <see cref="IWebSocketStream"/>, but no logger.
        /// </summary>
        public CandlestickWebSocketClientManager()
            : this(new CandlestickClient(), new WebSocketStreamController(new BinanceWebSocketStream()))
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
