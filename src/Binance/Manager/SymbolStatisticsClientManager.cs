using System;
using Binance.Client;
using Binance.Client.Events;
using Binance.Stream;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
{
    /// <summary>
    /// The default <see cref="ISymbolStatisticsClientManager"/> implementation.
    /// </summary>
    public class SymbolStatisticsClientManager : SymbolStatisticsClientManager<IJsonStream>, ISymbolStatisticsClientManager
    {
        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ISymbolStatisticsClient"/>,
        /// but no logger.
        /// </summary>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public SymbolStatisticsClientManager(IJsonStreamController controller, ILogger<SymbolStatisticsClientManager> logger = null)
            : this(new SymbolStatisticsClient(), controller, logger)
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public SymbolStatisticsClientManager(ISymbolStatisticsClient client, IJsonStreamController controller, ILogger<SymbolStatisticsClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors
    }

    public class SymbolStatisticsClientManager<TStream> : JsonStreamClientManager<ISymbolStatisticsClient, TStream>, ISymbolStatisticsClientManager<TStream>
        where TStream : IJsonStream
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
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public SymbolStatisticsClientManager(ISymbolStatisticsClient client, IJsonStreamController<TStream> controller, ILogger<SymbolStatisticsClientManager<TStream>> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public virtual void Subscribe(Action<SymbolStatisticsEventArgs> callback)
            => HandleSubscribe(() => Client.Subscribe(callback));

        public virtual void Subscribe(string symbol, Action<SymbolStatisticsEventArgs> callback)
            => HandleSubscribe(() => Client.Subscribe(symbol, callback));


        public virtual void Unsubscribe(Action<SymbolStatisticsEventArgs> callback)
            => HandleUnsubscribe(() => Client.Unsubscribe(callback));

        public virtual void Unsubscribe(string symbol, Action<SymbolStatisticsEventArgs> callback)
            => HandleUnsubscribe(() => Client.Unsubscribe(symbol, callback));

        #endregion Public Methods
    }
}
