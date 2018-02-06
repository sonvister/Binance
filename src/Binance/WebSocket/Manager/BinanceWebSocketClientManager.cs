using System;
using System.Collections.Generic;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// Multiple <see cref="IBinanceWebSocketClient"/> controller with automatic stream reconnect.
    /// </summary>
    public sealed class BinanceWebSocketClientManager : IBinanceWebSocketClientManager, IDisposable
    {
        public event EventHandler<BinanceWebSocketClientManagerErrorEventArgs> Error;

        public IAggregateTradeWebSocketClient AggregateTradeClient => _aggregateTradeClientAdapter;

        public ICandlestickWebSocketClient CandlestickClient => _candlestickClientAdapter;

        public IDepthWebSocketClient DepthClient => _depthClientAdapter;

        public ISymbolStatisticsWebSocketClient StatisticsClient => _statisticsClientAdapter;

        public ITradeWebSocketClient TradeClient => _tradeClientAdapter;

        public bool IsAutoStreamingDisabled { get; set; }

        #region Private Fields

        private readonly IAggregateTradeWebSocketClient _aggregateTradeClient;
        private readonly ICandlestickWebSocketClient _candlestickClient;
        private readonly IDepthWebSocketClient _depthClient;
        private readonly ISymbolStatisticsWebSocketClient _statisticsClient;
        private readonly ITradeWebSocketClient _tradeClient;

        private readonly AggregateTradeWebSocketClientAdapter _aggregateTradeClientAdapter;
        private readonly CandlestickWebSocketClientAdapter _candlestickClientAdapter;
        private readonly DepthWebSocketClientAdapter _depthClientAdapter;
        private readonly SymbolStatisticsWebSocketClientAdapter _statisticsClientAdapter;
        private readonly TradeWebSocketClientAdapter _tradeClientAdapter;

        private readonly ILogger<IBinanceWebSocketClientManager> _logger;

        private IDictionary<IWebSocketStream, WebSocketStreamController> _controllers
            = new Dictionary<IWebSocketStream, WebSocketStreamController>();

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        public BinanceWebSocketClientManager(
            IAggregateTradeWebSocketClient aggregateTradeClient,
            ICandlestickWebSocketClient candlestickClient,
            IDepthWebSocketClient depthClient,
            ISymbolStatisticsWebSocketClient statisticsClient,
            ITradeWebSocketClient tradeClient,
            ILogger<IBinanceWebSocketClientManager> logger = null)
        {
            _aggregateTradeClient = aggregateTradeClient;
            _candlestickClient = candlestickClient;
            _depthClient = depthClient;
            _statisticsClient = statisticsClient;
            _tradeClient = tradeClient;

            _logger = logger;


            _aggregateTradeClientAdapter = new AggregateTradeWebSocketClientAdapter(
                this, _aggregateTradeClient, _logger,
                err => RaiseErrorEvent(_aggregateTradeClientAdapter, err));

            _candlestickClientAdapter = new CandlestickWebSocketClientAdapter(
                this, _candlestickClient, _logger,
                err => RaiseErrorEvent(_candlestickClientAdapter, err));

            _depthClientAdapter = new DepthWebSocketClientAdapter(
                this, _depthClient, _logger,
                err => RaiseErrorEvent(_depthClientAdapter, err));

            _statisticsClientAdapter = new SymbolStatisticsWebSocketClientAdapter(
                this, _statisticsClient, _logger,
                err => RaiseErrorEvent(_statisticsClientAdapter, err));

            _tradeClientAdapter = new TradeWebSocketClientAdapter(
                this, _tradeClient, _logger,
                err => RaiseErrorEvent(_tradeClientAdapter, err));


            _controllers[_aggregateTradeClient.WebSocket] =
                new WebSocketStreamController(
                    _aggregateTradeClient.WebSocket,
                    err => RaiseErrorEvent(_aggregateTradeClientAdapter, err));

            _controllers[_candlestickClient.WebSocket] =
                new WebSocketStreamController(
                    _candlestickClient.WebSocket,
                    err => RaiseErrorEvent(_candlestickClientAdapter, err));

            _controllers[_depthClient.WebSocket] =
                new WebSocketStreamController(
                    _depthClient.WebSocket,
                    err => RaiseErrorEvent(_depthClientAdapter, err));

            _controllers[_statisticsClient.WebSocket] =
                new WebSocketStreamController(
                    _statisticsClient.WebSocket,
                    err => RaiseErrorEvent(_statisticsClientAdapter, err));

            _controllers[_tradeClient.WebSocket] =
                new WebSocketStreamController(
                    _tradeClient.WebSocket,
                    err => RaiseErrorEvent(_tradeClientAdapter, err));
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
            var args = new BinanceWebSocketClientManagerErrorEventArgs(
                new BinanceWebSocketClientManagerException(client, message, exception));

            try { Error?.Invoke(this, args); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _logger?.LogError(e, $"{nameof(BinanceWebSocketClientManager)}: Unhandled error event handler exception.");
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        void Dispose(bool disposing)
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
