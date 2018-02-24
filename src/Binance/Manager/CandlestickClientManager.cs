using System;
using Binance.Client;
using Binance.Client.Events;
using Binance.Market;
using Binance.Stream;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
{
    /// <summary>
    /// The default <see cref="ICandlestickClientManager"/> implementation.
    /// </summary>
    public class CandlestickClientManager : CandlestickClientManager<IJsonStream>, ICandlestickClientManager
    {
        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ICandlestickClient"/>,
        /// but no logger.
        /// </summary>
        /// <param name="controller">The JSON stream controller provider (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickClientManager(IJsonStreamController controller, ILogger<CandlestickClientManager> logger = null)
            : this(new CandlestickClient(), controller, logger)
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickClientManager(ICandlestickClient client, IJsonStreamController controller, ILogger<CandlestickClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors
    }

    public class CandlestickClientManager<TStream> : JsonStreamClientManager<ICandlestickClient, TStream>, ICandlestickClientManager<TStream>
        where TStream : IJsonStream
    {
        #region Public Events

        public event EventHandler<CandlestickEventArgs> Candlestick
        {
            add => Client.Candlestick += value;
            remove => Client.Candlestick -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickClientManager(ICandlestickClient client, IJsonStreamController<TStream> controller, ILogger<CandlestickClientManager<TStream>> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public virtual ICandlestickClient Subscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
            => (ICandlestickClient)HandleSubscribe(() => Client.Subscribe(symbol, interval, callback));

        public virtual ICandlestickClient Unsubscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
            => (ICandlestickClient)HandleUnsubscribe(() => Client.Unsubscribe(symbol, interval, callback));

        public virtual new ICandlestickClient Unsubscribe() => (ICandlestickClient)base.Unsubscribe();

        #endregion Public Methods
    }
}
