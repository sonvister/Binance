using System;
using System.Collections.Generic;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// Multiple <see cref="IBinanceWebSocketClient"/> controller with automatic stream reconnect.
    /// </summary>
    public sealed class BinanceWebSocketManager : IBinanceWebSocketManager
    {
        #region Public Events

        public event EventHandler<WebSocketManagerErrorEventArgs> Error;

        #endregion Public Events

        #region Public Properties

        public IAggregateTradeWebSocketClient AggregateTradeClient => _aggregateTradeClientAdapter;

        public ICandlestickWebSocketClient CandlestickClient => _candlestickClientAdapter;

        public IDepthWebSocketClient DepthClient => _depthClientAdapter;

        public ISymbolStatisticsWebSocketClient StatisticsClient => _statisticsClientAdapter;

        public ITradeWebSocketClient TradeClient => _tradeClientAdapter;

        public bool IsAutoStreamingDisabled { get; set; }

        #endregion Public Properties

        #region Private Fields

        private readonly AggregateTradeWebSocketClientAdapter _aggregateTradeClientAdapter;
        private readonly CandlestickWebSocketClientAdapter _candlestickClientAdapter;
        private readonly DepthWebSocketClientAdapter _depthClientAdapter;
        private readonly SymbolStatisticsWebSocketClientAdapter _statisticsClientAdapter;
        private readonly TradeWebSocketClientAdapter _tradeClientAdapter;

        private readonly ILogger<IBinanceWebSocketManager> _logger;

        private readonly IDictionary<IWebSocketStream, WebSocketStreamController> _controllers
            = new Dictionary<IWebSocketStream, WebSocketStreamController>();

        #endregion Private Fields

        #region Constructors

        public BinanceWebSocketManager(
            IAggregateTradeWebSocketClient aggregateTradeClient,
            ICandlestickWebSocketClient candlestickClient,
            IDepthWebSocketClient depthClient,
            ISymbolStatisticsWebSocketClient statisticsClient,
            ITradeWebSocketClient tradeClient,
            ILogger<IBinanceWebSocketManager> logger = null)
        {
            Throw.IfNull(aggregateTradeClient, nameof(aggregateTradeClient));
            Throw.IfNull(candlestickClient, nameof(candlestickClient));
            Throw.IfNull(depthClient, nameof(depthClient));
            Throw.IfNull(statisticsClient, nameof(statisticsClient));
            Throw.IfNull(tradeClient, nameof(tradeClient));

            _logger = logger;


            _aggregateTradeClientAdapter = new AggregateTradeWebSocketClientAdapter(
                this, aggregateTradeClient, _logger,
                err => RaiseErrorEvent(_aggregateTradeClientAdapter, err,
                    $"{nameof(IAggregateTradeWebSocketClient)}: Adapter failed."));

            _candlestickClientAdapter = new CandlestickWebSocketClientAdapter(
                this, candlestickClient, _logger,
                err => RaiseErrorEvent(_candlestickClientAdapter, err,
                    $"{nameof(ICandlestickWebSocketClient)}: Adapter failed."));

            _depthClientAdapter = new DepthWebSocketClientAdapter(
                this, depthClient, _logger,
                err => RaiseErrorEvent(_depthClientAdapter, err,
                    $"{nameof(IDepthWebSocketClient)}: Adapter failed."));

            _statisticsClientAdapter = new SymbolStatisticsWebSocketClientAdapter(
                this, statisticsClient, _logger,
                err => RaiseErrorEvent(_statisticsClientAdapter, err,
                    $"{nameof(ISymbolStatisticsWebSocketClient)}: Adapter failed."));

            _tradeClientAdapter = new TradeWebSocketClientAdapter(
                this, tradeClient, _logger,
                err => RaiseErrorEvent(_tradeClientAdapter, err,
                    $"{nameof(ITradeWebSocketClient)}: Adapter failed."));


            _controllers[aggregateTradeClient.WebSocket] =
                new WebSocketStreamController(
                    aggregateTradeClient.WebSocket,
                    err => RaiseErrorEvent(_aggregateTradeClientAdapter, err,
                        $"{nameof(IAggregateTradeWebSocketClient)}: Controller failed."));

            _controllers[candlestickClient.WebSocket] =
                new WebSocketStreamController(
                    candlestickClient.WebSocket,
                    err => RaiseErrorEvent(_candlestickClientAdapter, err,
                        $"{nameof(ICandlestickWebSocketClient)}: Controller failed."));

            _controllers[depthClient.WebSocket] =
                new WebSocketStreamController(
                    depthClient.WebSocket,
                    err => RaiseErrorEvent(_depthClientAdapter, err,
                        $"{nameof(IDepthWebSocketClient)}: Controller failed."));

            _controllers[statisticsClient.WebSocket] =
                new WebSocketStreamController(
                    statisticsClient.WebSocket,
                    err => RaiseErrorEvent(_statisticsClientAdapter, err,
                        $"{nameof(ISymbolStatisticsWebSocketClient)}: Controller failed."));

            _controllers[tradeClient.WebSocket] =
                new WebSocketStreamController(
                    tradeClient.WebSocket,
                    err => RaiseErrorEvent(_tradeClientAdapter, err,
                        $"{nameof(ITradeWebSocketClient)}: Controller failed."));
        }

        #endregion Constructors

        #region Public Methods

        public IRetryTaskController GetController(IWebSocketStream webSocket)
            => _controllers.ContainsKey(webSocket) ? _controllers[webSocket] : null;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Raise an error event on controller exception.
        /// </summary>
        /// <param name="client">The client associated with the exception.</param>
        /// <param name="exception">The inner exception.</param>
        /// <param name="message">The exception message (optional).</param>
        private void RaiseErrorEvent(IBinanceWebSocketClient client, Exception exception, string message = null)
        {
            var args = new WebSocketManagerErrorEventArgs(
                new WebSocketManagerException(client, message, exception));

            try { Error?.Invoke(this, args); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _logger?.LogError(e, $"{nameof(BinanceWebSocketManager)}: Unhandled error event handler exception.");
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                this.CancelAllAsync().GetAwaiter().GetResult();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
