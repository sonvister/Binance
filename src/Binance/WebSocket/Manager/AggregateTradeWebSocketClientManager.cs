using Binance.Client;
using Binance.Manager;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// The default <see cref="IAggregateTradeWebSocketClientManager"/> implementation.
    /// </summary>
    public class AggregateTradeWebSocketClientManager : AggregateTradeClientManager<IWebSocketStream>, IAggregateTradeWebSocketClientManager
    {
        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IAggregateTradeClient"/>
        /// and default <see cref="IWebSocketStream"/>, but no logger.
        /// </summary>
        public AggregateTradeWebSocketClientManager()
            : this(new AggregateTradeClient(), new BinanceWebSocketStreamController())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The web socket stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public AggregateTradeWebSocketClientManager(IAggregateTradeClient client, IWebSocketStreamController controller, ILogger<AggregateTradeWebSocketClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Construtors
    }
}
