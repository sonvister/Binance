using System;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ISymbolStatisticsWebSocketClient"/> implementation.
    /// </summary>
    public class SymbolStatisticsWebSocketClient : BinanceWebSocketClient<IWebSocketStream, ISymbolStatisticsClient, SymbolStatisticsEventArgs>, ISymbolStatisticsWebSocketClient
    {
        #region Public Events

        public event EventHandler<SymbolStatisticsEventArgs> StatisticsUpdate
        {
            add => Client.StatisticsUpdate += value;
            remove => Client.StatisticsUpdate -= value;
        }

        public event EventHandler<ErrorEventArgs> Error
        {
            add => Publisher.Controller.Error += value;
            remove => Publisher.Controller.Error -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ISymbolStatisticsClient"/>
        /// and default <see cref="IBinanceWebSocketStreamPublisher"/>, but no logger.
        /// </summary>
        public SymbolStatisticsWebSocketClient()
            : this(new SymbolStatisticsClient(), new BinanceWebSocketStreamPublisher())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="publisher">The web socket stream publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public SymbolStatisticsWebSocketClient(ISymbolStatisticsClient client, IBinanceWebSocketStreamPublisher publisher, ILogger<SymbolStatisticsWebSocketClient> logger = null)
            : base(client, publisher, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual ISymbolStatisticsClient Subscribe(Action<SymbolStatisticsEventArgs> callback, params string[] symbols)
            => (ISymbolStatisticsClient)HandleSubscribe(() => Client.Subscribe(callback, symbols));

        public virtual ISymbolStatisticsClient Unsubscribe(Action<SymbolStatisticsEventArgs> callback, params string[] symbols)
            => (ISymbolStatisticsClient)HandleUnsubscribe(() => Client.Unsubscribe(callback, symbols));

        #endregion Public Methods
    }
}
