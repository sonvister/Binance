using System;
using Binance.Client;
using Binance.Producer;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The Binance web socket client abstract base class.
    /// </summary>
    public abstract class BinanceWebSocketClient<TStream, TClient, TEventArgs> : JsonPublisherClient<TClient, IAutoJsonStreamPublisher<TStream>>
        where TStream : IWebSocketStream
        where TClient : IJsonClient
        where TEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The JSON stream publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected BinanceWebSocketClient(TClient client, IAutoJsonStreamPublisher<TStream> publisher, ILogger<BinanceWebSocketClient<TStream, TClient, TEventArgs>> logger = null)
            : base(client, publisher, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public virtual new TClient Unsubscribe() => (TClient)base.Unsubscribe();

        #endregion Public Methods
    }
}
