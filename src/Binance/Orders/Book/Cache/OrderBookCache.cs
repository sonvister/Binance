using Binance.Api.WebSocket;
using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Binance.Orders.Book.Cache
{
    public class OrderBookCache : OrderBook, IOrderBookCache
    {
        #region Public Events

        public event EventHandler<OrderBookUpdateEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Get last update ID (synchronized).
        /// </summary>
        public override long LastUpdateId
        {
            get
            {
                lock (_sync) return base.LastUpdateId;
            }
        }

        /// <summary>
        /// Get order book top (synchronized) if availabie, otherwise null.
        /// </summary>
        public override OrderBookTop Top
        {
            get
            {
                lock (_sync) return LastUpdateId > 0 ? base.Top : null;
            }
        }

        /// <summary>
        /// Get bids (synchronized) if available, otherwise empty enumerable.
        /// </summary>
        public override IEnumerable<OrderBookPriceLevel> Bids
        {
            get
            {
                lock (_sync) return LastUpdateId > 0 ? base.Bids : new OrderBookPriceLevel[] { };
            }
        }

        /// <summary>
        /// Get asks (synchronized) if available, otherwise empty enumerable.
        /// </summary>
        public override IEnumerable<OrderBookPriceLevel> Asks
        {
            get
            {
                lock (_sync) return LastUpdateId > 0 ? base.Asks : new OrderBookPriceLevel[] { };
            }
        }

        public IDepthWebSocketClient Client { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private IBinanceApi _api;

        private ILogger<OrderBookCache> _logger;

        private bool _leaveWebSocketClientOpen;

        private BufferBlock<DepthUpdateEventArgs> _bufferBlock;
        private ActionBlock<DepthUpdateEventArgs> _actionBlock;

        private Action<OrderBookUpdateEventArgs> _callback;

        private readonly object _sync = new object();

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

        public Task SubscribeAsync(string symbol, Action<OrderBookUpdateEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol.FormatSymbol();

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
                    if (LastUpdateId == 0)
                    {
                        var orderBook = await _api.GetOrderBookAsync(Symbol)
                            .ConfigureAwait(false);

                        // Modify this order book cache.
                        if (orderBook != null)
                        {
                            Symbol = orderBook.Symbol;
                            Modify(orderBook.LastUpdateId, orderBook.Bids.Select(b => (b.Price, b.Quantity)), orderBook.Asks.Select(a => (a.Price, a.Quantity)));
                        }
                    }

                    if (@event.FirstUpdateId > LastUpdateId + 1)
                    {
                        _logger?.LogError($"{nameof(DepthWebSocketClient)}: Synchronization failure (first update ID > last update ID + 1).");

                        await Task.Delay(1000)
                            .ConfigureAwait(false); // wait a bit.

                        var orderBook = await _api.GetOrderBookAsync(Symbol)
                            .ConfigureAwait(false);

                        if (orderBook != null)
                            Modify(orderBook.LastUpdateId, orderBook.Bids.Select(b => (b.Price, b.Quantity)), orderBook.Asks.Select(a => (a.Price, a.Quantity)));
                    }
                    // NOTE: This does not handle re-synchronizing if still unsynchronized...

                    Modify(@event.LastUpdateId, @event.Bids, @event.Asks);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(DepthWebSocketClient)}: \"{e.Message}\"");
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
        /// Raise depth of market update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(OrderBookUpdateEventArgs args)
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
        protected override void Modify(long lastUpdateId, IEnumerable<(decimal, decimal)> bids, IEnumerable<(decimal, decimal)> asks)
        {
            if (lastUpdateId < LastUpdateId)
                return;

            lock (_sync)
            {
                Modify(lastUpdateId, bids, asks);
            }

            var eventArgs = new OrderBookUpdateEventArgs(Clone());

            _callback?.Invoke(eventArgs);
            RaiseUpdateEvent(eventArgs);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// <see cref="DepthWebSocketClient"/> event handler.
        /// </summary>
        /// <param name="sender">The <see cref="DepthWebSocketClient"/>.</param>
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

        #region ICloneable

        public override IOrderBook Clone(int limit = default)
        {
            lock (_sync)
            {
                return base.Clone(limit);
            }
        }

        #endregion ICloneable

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
