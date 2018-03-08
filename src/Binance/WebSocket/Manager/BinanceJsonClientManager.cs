using System;
using Binance.Client;
using Binance.Stream;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IBinanceJsonClientManager"/> implementation.
    /// </summary>
    public abstract class BinanceJsonClientManager<TStream> : IBinanceJsonClientManager
        where TStream : IJsonStream
    {
        #region Public Events

        public event EventHandler<ErrorEventArgs> Error;

        #endregion Public Events

        #region Public Properties

        public IAggregateTradeClient AggregateTradeClient { get; }

        public ICandlestickClient CandlestickClient { get; }

        public IDepthClient DepthClient { get; }

        public ISymbolStatisticsClient StatisticsClient { get; }

        public ITradeClient TradeClient { get; }

        #endregion Public Properties

        #region Private Properties

        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<BinanceJsonClientManager<TStream>> _logger;

        #endregion Private Properties

        #region Constructors

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="aggregateTradeClient"></param>
        /// <param name="candlestickClient"></param>
        /// <param name="depthClient"></param>
        /// <param name="statisticsClient"></param>
        /// <param name="tradeClient"></param>
        /// <param name="logger"></param>
        protected BinanceJsonClientManager(
            IAggregateTradeWebSocketClient aggregateTradeClient,
            ICandlestickWebSocketClient candlestickClient,
            IDepthWebSocketClient depthClient,
            ISymbolStatisticsWebSocketClient statisticsClient,
            ITradeWebSocketClient tradeClient,
            ILogger<BinanceJsonClientManager<TStream>> logger = null)
        {
            Throw.IfNull(aggregateTradeClient, nameof(aggregateTradeClient));
            Throw.IfNull(candlestickClient, nameof(candlestickClient));
            Throw.IfNull(depthClient, nameof(depthClient));
            Throw.IfNull(statisticsClient, nameof(statisticsClient));
            Throw.IfNull(tradeClient, nameof(tradeClient));

            AggregateTradeClient = aggregateTradeClient;
            CandlestickClient = candlestickClient;
            DepthClient = depthClient;
            StatisticsClient = statisticsClient;
            TradeClient = tradeClient;
            _logger = logger;

            // Forward controller error events.
            aggregateTradeClient.Error += HandleError;
            candlestickClient.Error += HandleError;
            depthClient.Error += HandleError;
            statisticsClient.Error += HandleError;
            tradeClient.Error += HandleError;
        }

        #endregion Constructors

        #region Protected Methods

        /// <summary>
        /// Raise an error event.
        /// </summary>
        /// <param name="exception"></param>
        protected void OnError(Exception exception)
        {
            try { Error?.Invoke(this, new ErrorEventArgs(exception)); }
            catch (Exception) { /* ignore */ }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handle controller error.
        /// 
        /// NOTE: Use a method to allow event double-bind protection keep from
        ///       subscribing to the same controller multiple times.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleError(object sender, ErrorEventArgs e)
        {
            OnError(e.Exception);
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                AggregateTradeClient.Unsubscribe();
                CandlestickClient.Unsubscribe();
                DepthClient.Unsubscribe();
                StatisticsClient.Unsubscribe();
                TradeClient.Unsubscribe();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
