using System;
using Binance.Client;
using Binance.Stream;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The Binance web socket client abstract base class.
    /// </summary>
    public abstract class AutoBinanceWebSocketClient<TStream, TClient, TEventArgs> : JsonPublisherClient<TClient, IAutoJsonStreamPublisher<TStream>>
        where TStream : IWebSocketStream
        where TClient : IJsonClient
        where TEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="publisher">The JSON stream publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected AutoBinanceWebSocketClient(TClient client, IAutoJsonStreamPublisher<TStream> publisher, ILogger<AutoBinanceWebSocketClient<TStream, TClient, TEventArgs>> logger = null)
            : base(client, publisher, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public new virtual TClient Unsubscribe() => (TClient)base.Unsubscribe();

        #endregion Public Methods
    }
}
