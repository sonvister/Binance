using System;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IDepthWebSocketClient"/> implementation.
    /// </summary>
    public class DepthWebSocketClient : BinanceWebSocketClient<IDepthClient, DepthUpdateEventArgs>, IDepthWebSocketClient
    {
        #region Public Events

        public event EventHandler<DepthUpdateEventArgs> DepthUpdate
        {
            add => Client.DepthUpdate += value;
            remove => Client.DepthUpdate -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IDepthClient"/>
        /// and default <see cref="IBinanceWebSocketStream"/>, but no logger.
        /// </summary>
        public DepthWebSocketClient()
            : this(new DepthClient(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The web socket stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public DepthWebSocketClient(IDepthClient client, IBinanceWebSocketStream stream, ILogger<DepthWebSocketClient> logger = null)
            : base(client, stream, logger)
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
