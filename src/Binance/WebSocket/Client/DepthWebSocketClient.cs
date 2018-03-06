using System;
using Binance.Client;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IDepthWebSocketClient"/> implementation.
    /// </summary>
    public class DepthWebSocketClient : AutoBinanceWebSocketClient<IWebSocketStream, IDepthClient, DepthUpdateEventArgs>, IDepthWebSocketClient
    {
        #region Public Events

        public event EventHandler<DepthUpdateEventArgs> DepthUpdate
        {
            add => Client.DepthUpdate += value;
            remove => Client.DepthUpdate -= value;
        }

        public event EventHandler<ErrorEventArgs> Error
        {
            add => Publisher.Controller.Error += value;
            remove => Publisher.Controller.Error -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IDepthClient"/>
        /// and default <see cref="IBinanceWebSocketStreamPublisher"/>, but no logger.
        /// </summary>
        public DepthWebSocketClient()
            : this(new DepthClient(), new BinanceWebSocketStreamPublisher())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="publisher">The web socket stream publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public DepthWebSocketClient(IDepthClient client, IBinanceWebSocketStreamPublisher publisher, ILogger<DepthWebSocketClient> logger = null)
            : base(client, publisher, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual IDepthClient Subscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
            => (IDepthClient)HandleSubscribe(() => Client.Subscribe(symbol, limit, callback));

        public virtual IDepthClient Unsubscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
            => (IDepthClient)HandleUnsubscribe(() => Client.Unsubscribe(symbol, limit, callback));

        #endregion Public Methods
    }
}
