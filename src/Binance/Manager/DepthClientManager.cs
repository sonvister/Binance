using System;
using Binance.Client;
using Binance.Client.Events;
using Binance.Stream;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
{
    /// <summary>
    /// The default <see cref="IDepthClientManager"/> implementation.
    /// </summary>
    public class DepthClientManager : DepthClientManager<IJsonStream>, IDepthClientManager
    {
        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IDepthClient"/>,
        /// but no logger.
        /// </summary>
        /// <param name="controller">The JSON stream controller provider (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public DepthClientManager(IJsonStreamController controller, ILogger<DepthClientManager> logger = null)
            : this(new DepthClient(), controller, logger)
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public DepthClientManager(IDepthClient client, IJsonStreamController controller, ILogger<DepthClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors
    }

    public class DepthClientManager<TStream> : JsonStreamClientManager<IDepthClient, TStream>, IDepthClientManager<TStream>
        where TStream : IJsonStream
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
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public DepthClientManager(IDepthClient client, IJsonStreamController<TStream> controller, ILogger<DepthClientManager<TStream>> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public virtual void Subscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
            => HandleSubscribe(() => Client.Subscribe(symbol, limit, callback));

        public virtual void Unsubscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
            => HandleUnsubscribe(() => Client.Unsubscribe(symbol, limit, callback));

        #endregion Public Methods
    }
}
