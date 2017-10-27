using Binance.Api.WebSocket;
using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Binance.Orders.Book.Cache
{
    public class OrderBookCache : IOrderBookCache
    {
        #region Public Events

        public event EventHandler<OrderBookCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        public OrderBook OrderBook => _orderBookClone;

        public IDepthWebSocketClient Client { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private IBinanceApi _api;

        private ILogger<OrderBookCache> _logger;

        private bool _leaveWebSocketClientOpen;

        private BufferBlock<DepthUpdateEventArgs> _bufferBlock;
        private ActionBlock<DepthUpdateEventArgs> _actionBlock;

        private Action<OrderBookCacheEventArgs> _callback;

        private OrderBook _orderBook;
        private OrderBook _orderBookClone;

        #endregion Private Fields

        #region Constructors

        public OrderBookCache(IBinanceApi api, IDepthWebSocketClient client, bool leaveWebSocketClientOpen = false, ILogger<OrderBookCache> logger = null)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(client, nameof(client));

            _api = api;
            _logger = logger;

            Client = client;
            Client.DepthUpdate += OnDepthUpdate;
            _leaveWebSocketClientOpen = leaveWebSocketClientOpen;
        }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(string symbol, CancellationToken token = default)
            => SubscribeAsync(symbol, null, token);

        public Task SubscribeAsync(string symbol, Action<OrderBookCacheEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            _callback = callback;

            _bufferBlock = new BufferBlock<DepthUpdateEventArgs>(new DataflowBlockOptions()
            {
                EnsureOrdered = true,
                CancellationToken = token,
                BoundedCapacity = DataflowBlockOptions.Unbounded,
                MaxMessagesPerTask = DataflowBlockOptions.Unbounded,
            });

            _actionBlock = new ActionBlock<DepthUpdateEventArgs>(async @event =>
            {
                try
                {
                    // If order book has not been initialized.
                    if (_orderBook == null)
                    {
                        _orderBook = await _api.GetOrderBookAsync(symbol, token: token)
                            .ConfigureAwait(false);
                    }

                    // If there is a gap in events (order book out-of-sync).
                    if (@event.FirstUpdateId > _orderBook.LastUpdateId + 1)
                    {
                        _logger?.LogError($"{nameof(OrderBookCache)}: Synchronization failure (first update ID > last update ID + 1).");

                        await Task.Delay(1000)
                            .ConfigureAwait(false); // wait a bit.

                        _orderBook = await _api.GetOrderBookAsync(symbol)
                            .ConfigureAwait(false);
                    }
                    // NOTE: This does not handle re-synchronizing if still out-of-sync...

                    Modify(@event.LastUpdateId, @event.Bids, @event.Asks);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(OrderBookCache)}: \"{e.Message}\"");
                }
            }, new ExecutionDataflowBlockOptions()
            {
                BoundedCapacity = 1,
                EnsureOrdered = true,
                MaxDegreeOfParallelism = 1,
                CancellationToken = token,
                SingleProducerConstrained = true,
            });

            _bufferBlock.LinkTo(_actionBlock);

            return Client.SubscribeAsync(symbol, token: token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Raise order book cache update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(OrderBookCacheEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { Update?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(DepthWebSocketClient)}.{nameof(RaiseUpdateEvent)}");
                throw;
            }
        }

        /// <summary>
        /// Update the order book.
        /// </summary>
        /// <param name="lastUpdateId"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        protected virtual void Modify(long lastUpdateId, IEnumerable<(decimal, decimal)> bids, IEnumerable<(decimal, decimal)> asks)
        {
            if (lastUpdateId < _orderBook.LastUpdateId)
                return;

            _orderBook.Modify(lastUpdateId, bids, asks);

            _orderBookClone = _orderBook.Clone();

            var eventArgs = new OrderBookCacheEventArgs(_orderBookClone);

            _callback?.Invoke(eventArgs);
            RaiseUpdateEvent(eventArgs);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// <see cref="IDepthWebSocketClient"/> event handler.
        /// </summary>
        /// <param name="sender">The <see cref="IDepthWebSocketClient"/>.</param>
        /// <param name="event">The event arguments.</param>
        private void OnDepthUpdate(object sender, DepthUpdateEventArgs @event)
        {
            // Post event to buffer block (queue).
            _bufferBlock.Post(@event);
        }

        /// <summary>
        /// Log an exception if not already logged within this library.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        private void LogException(Exception e, string source)
        {
            if (!e.IsLogged())
            {
                _logger?.LogError(e, $"{source}: \"{e.Message}\"");
                e.Logged();
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Client.DepthUpdate -= OnDepthUpdate;

                if (!_leaveWebSocketClientOpen)
                {
                    Client.Dispose();
                }

                _bufferBlock?.Complete();
                _actionBlock?.Complete();
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
