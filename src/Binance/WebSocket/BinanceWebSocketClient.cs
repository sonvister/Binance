using System;
using Binance.Client;
using Binance.Stream;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The Binance web socket client abstract base class.
    /// </summary>
    public abstract class BinanceWebSocketClient<TClient, TEventArgs> : JsonStreamClient<TClient, IBinanceWebSocketStream>, IBinanceWebSocketClient
        where TClient : IJsonClient
        where TEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The web socket stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected BinanceWebSocketClient(TClient client, IBinanceWebSocketStream stream, ILogger<BinanceWebSocketClient<TClient, TEventArgs>> logger = null)
            : base(client, stream, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public virtual new TClient Unsubscribe() => (TClient)base.Unsubscribe();

        #endregion Public Methods
    }
}
