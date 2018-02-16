using System;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ISymbolStatisticsWebSocketClient"/> implementation.
    /// </summary>
    public class SymbolStatisticsWebSocketClient : BinanceWebSocketClient<ISymbolStatisticsClient, SymbolStatisticsEventArgs>, ISymbolStatisticsWebSocketClient
    {
        #region Public Events

        public event EventHandler<SymbolStatisticsEventArgs> StatisticsUpdate
        {
            add => Client.StatisticsUpdate += value;
            remove => Client.StatisticsUpdate -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ISymbolStatisticsClient"/>
        /// and default <see cref="IBinanceWebSocketStream"/>, but no logger.
        /// </summary>
        public SymbolStatisticsWebSocketClient()
            : this(new SymbolStatisticsClient(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The web socket stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public SymbolStatisticsWebSocketClient(ISymbolStatisticsClient client, IBinanceWebSocketStream stream, ILogger<SymbolStatisticsWebSocketClient> logger = null)
            : base(client, stream, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe(Action<SymbolStatisticsEventArgs> callback)
            => HandleSubscribe(() => Client.Subscribe(callback));

        public virtual void Unsubscribe(Action<SymbolStatisticsEventArgs> callback)
            => HandleUnsubscribe(() => Client.Unsubscribe(callback));

        public virtual void Subscribe(string symbol, Action<SymbolStatisticsEventArgs> callback)
            => HandleSubscribe(() => Client.Subscribe(symbol, callback));

        public virtual void Unsubscribe(string symbol, Action<SymbolStatisticsEventArgs> callback)
            => HandleUnsubscribe(() => Client.Unsubscribe(symbol, callback));

        #endregion Public Methods
    }
}
