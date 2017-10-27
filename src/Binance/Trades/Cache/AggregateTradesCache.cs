using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Binance.Trades.Cache
{
    public class AggregateTradesCache : IAggregateTradesCache
    {
        #region Public Events

        public event EventHandler<AggregateTradesCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        public IEnumerable<AggregateTrade> Trades
        {
            get { lock (_sync) { return _trades?.ToArray() ?? new AggregateTrade[] { }; } }
        }

        public ITradesWebSocketClient Client { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private IBinanceApi _api;

        private ILogger<AggregateTradesCache> _logger;

        private bool _leaveWebSocketClientOpen;

        private BufferBlock<AggregateTradeEventArgs> _bufferBlock;
        private ActionBlock<AggregateTradeEventArgs> _actionBlock;

        private Action<AggregateTradesCacheEventArgs> _callback;

        private Queue<AggregateTrade> _trades;

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        public AggregateTradesCache(IBinanceApi api, ITradesWebSocketClient client, bool leaveWebSocketClientOpen = false, ILogger<AggregateTradesCache> logger = null)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(client, nameof(client));

            _api = api;
            _logger = logger;

            Client = client;
            Client.AggregateTrade += OnAggregateTrade;
            _leaveWebSocketClientOpen = leaveWebSocketClientOpen;

            _trades = new Queue<AggregateTrade>();
        }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(string symbol, int limit = default, CancellationToken token = default)
            => SubscribeAsync(symbol, null, limit, token);

        public Task SubscribeAsync(string symbol, Action<AggregateTradesCacheEventArgs> callback, int limit = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            _callback = callback;

            _bufferBlock = new BufferBlock<AggregateTradeEventArgs>(new DataflowBlockOptions()
            {
                EnsureOrdered = true,
                CancellationToken = token,
                BoundedCapacity = DataflowBlockOptions.Unbounded,
                MaxMessagesPerTask = DataflowBlockOptions.Unbounded,
            });

            _actionBlock = new ActionBlock<AggregateTradeEventArgs>(async @event =>
            {
                try
                {
                    if (_trades.Count == 0)
                    {
                        await SynchronizeTradesAsync(symbol, limit: limit, token: token)
                            .ConfigureAwait(false);
                    }

                    // If there is a gap in the trades received (out-of-sync).
                    if (@event.Trade.Id > _trades.Last().Id + 1)
                    {
                        _logger?.LogError($"{nameof(AggregateTradesCache)}: Synchronization failure (trade ID > last trade ID + 1).");

                        await Task.Delay(1000)
                            .ConfigureAwait(false); // wait a bit.

                        // Re-synchronize.
                        await SynchronizeTradesAsync(symbol, limit: limit, token: token)
                            .ConfigureAwait(false);

                        // If still out-of-sync.
                        if (@event.Trade.Id > _trades.Last().Id + 1)
                        {
                            _logger?.LogError($"{nameof(AggregateTradesCache)}: Re-Synchronization failure (trade ID > last trade ID + 1).");

                            // Reset and wait for next event.
                            lock (_sync) _trades.Clear();
                            return;
                        }
                    }

                    // If the trade exists in the queue already (occurs after synchronization).
                    if (_trades.Any(t => t.Id == @event.Trade.Id))
                        return;

                    lock (_sync)
                    {
                        _trades.Dequeue();
                        _trades.Enqueue(@event.Trade);
                    }

                    var eventArgs = new AggregateTradesCacheEventArgs(_trades.ToArray());

                    _callback?.Invoke(eventArgs);
                    RaiseUpdateEvent(eventArgs);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(AggregateTradesCache)}: \"{e.Message}\"");
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

            return Client.SubscribeAsync(symbol, token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Raise account cache update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(AggregateTradesCacheEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { Update?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(AggregateTradesCache)}.{nameof(RaiseUpdateEvent)}");
                throw;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Get latest trades.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task SynchronizeTradesAsync(string symbol, int limit, CancellationToken token)
        {
            var trades = await _api.GetAggregateTradesAsync(symbol, limit: limit, token: token)
                .ConfigureAwait(false);

            lock (_sync)
            {
                _trades.Clear();
                foreach (var trade in trades)
                {
                    _trades.Enqueue(trade);
                }
            }
        }

        /// <summary>
        /// <see cref="ITradesWebSocketClient"/> event handler.
        /// </summary>
        /// <param name="sender">The <see cref="ITradesWebSocketClient"/>.</param>
        /// <param name="event">The event arguments.</param>
        private void OnAggregateTrade(object sender, AggregateTradeEventArgs @event)
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
                Client.AggregateTrade -= OnAggregateTrade;

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
