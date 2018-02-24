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

        public virtual ISymbolStatisticsClient Subscribe(Action<SymbolStatisticsEventArgs> callback, params string[] symbols)
            => (ISymbolStatisticsClient)HandleSubscribe(() => Client.Subscribe(callback, symbols));

        public virtual ISymbolStatisticsClient Unsubscribe(Action<SymbolStatisticsEventArgs> callback, params string[] symbols)
            => (ISymbolStatisticsClient)HandleUnsubscribe(() => Client.Unsubscribe(callback, symbols));

        public virtual new ISymbolStatisticsClient Unsubscribe() => (ISymbolStatisticsClient)base.Unsubscribe();

        #endregion Public Methods
    }
}
