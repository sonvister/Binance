using System;
using Binance.Client;
using Binance.Stream;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
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

        private readonly ILogger<BinanceJsonClientManager<TStream>> _logger;

        #endregion Private Properties

        #region Constructors

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="aggregateTradeClientManager"></param>
        /// <param name="candlestickClientManager"></param>
        /// <param name="depthClientManager"></param>
        /// <param name="statisticsClientManager"></param>
        /// <param name="tradeClientManager"></param>
        /// <param name="logger"></param>
        protected BinanceJsonClientManager(
            IAggregateTradeClientManager<TStream> aggregateTradeClientManager,
            ICandlestickClientManager<TStream> candlestickClientManager,
            IDepthClientManager<TStream> depthClientManager,
            ISymbolStatisticsClientManager<TStream> statisticsClientManager,
            ITradeClientManager<TStream> tradeClientManager,
            ILogger<BinanceJsonClientManager<TStream>> logger = null)
        {
            Throw.IfNull(aggregateTradeClientManager, nameof(aggregateTradeClientManager));
            Throw.IfNull(candlestickClientManager, nameof(candlestickClientManager));
            Throw.IfNull(depthClientManager, nameof(depthClientManager));
            Throw.IfNull(statisticsClientManager, nameof(statisticsClientManager));
            Throw.IfNull(tradeClientManager, nameof(tradeClientManager));

            AggregateTradeClient = aggregateTradeClientManager;
            CandlestickClient = candlestickClientManager;
            DepthClient = depthClientManager;
            StatisticsClient = statisticsClientManager;
            TradeClient = tradeClientManager;
            _logger = logger;

            aggregateTradeClientManager.Controller.Error += HandleError;
            candlestickClientManager.Controller.Error += HandleError;
            depthClientManager.Controller.Error += HandleError;
            statisticsClientManager.Controller.Error += HandleError;
            tradeClientManager.Controller.Error += HandleError;
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
                (AggregateTradeClient as IDisposable)?.Dispose();
                (CandlestickClient as IDisposable)?.Dispose();
                (DepthClient as IDisposable)?.Dispose();
                (StatisticsClient as IDisposable)?.Dispose();
                (TradeClient as IDisposable)?.Dispose();
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
