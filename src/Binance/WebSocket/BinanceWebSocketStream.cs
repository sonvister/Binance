using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.WebSocket
{
    public sealed class BinanceWebSocketStream : IWebSocketStream
    {
        #region Public Events

        public event EventHandler<EventArgs> Open
        {
            add { Client.Open += value; }
            remove { Client.Open -= value; }
        }

        public event EventHandler<EventArgs> Close
        {
            add { Client.Close += value; }
            remove { Client.Close -= value; }
        }

        #endregion Public Events

        #region Public Properties

        public IWebSocketClient Client { get; }

        public IEnumerable<string> SubscribedStreams => _subscribers.Keys;

        public bool IsCombined => _subscribers.Keys.Count > 1;

        #endregion Public Properties

        #region Private Constants

        private const string BaseUri = "wss://stream.binance.com:9443";

        #endregion Private Constants

        #region Private Fields

        private int _maxBufferCount;

        private BufferBlock<string> _bufferBlock;
        private ActionBlock<string> _actionBlock;

        private readonly ILogger<BinanceWebSocketStream> _logger;

        private readonly IDictionary<string, ICollection<Action<WebSocketStreamEventArgs>>> _subscribers;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default web socket client, but no logging.
        /// </summary>
        public BinanceWebSocketStream()
            : this(new DefaultWebSocketClient())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger">The logger (optional).</param>
        public BinanceWebSocketStream(IWebSocketClient client, ILogger<BinanceWebSocketStream> logger = null)
        {
            Throw.IfNull(client, nameof(client));

            Client = client;
            _logger = logger;

            _subscribers = new Dictionary<string, ICollection<Action<WebSocketStreamEventArgs>>>();
        }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string stream, Action<WebSocketStreamEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            _logger?.LogDebug($"{nameof(BinanceWebSocketStream)}.{nameof(Subscribe)}: \"{stream}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            if (!_subscribers.ContainsKey(stream))
            {
                if (Client.IsStreaming)
                {
                    _logger?.LogWarning($"{nameof(BinanceWebSocketStream)}.{nameof(Subscribe)}: {nameof(IWebSocketClient)} is already streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                }

                _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(Subscribe)}: Adding stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                _subscribers[stream] = new List<Action<WebSocketStreamEventArgs>>();
            }

            if (!_subscribers[stream].Contains(callback))
            {
                _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(Subscribe)}: Adding callback for stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                _subscribers[stream].Add(callback);
            }
        }

        public void Unsubscribe(string stream, Action<WebSocketStreamEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            _logger?.LogDebug($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: \"{stream}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            if (!_subscribers.ContainsKey(stream))
            {
                _logger?.LogWarning($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: Not subscribed to stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                return; // ignore.
            }

            if (_subscribers[stream].Contains(callback))
            {
                _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: Removing callback for stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                _subscribers[stream].Remove(callback);
            }

            if (!_subscribers[stream].Any())
            {
                if (Client.IsStreaming)
                {
                    _logger?.LogWarning($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: {nameof(IWebSocketClient)} is already streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                }

                _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: Removing stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                _subscribers.Remove(stream);
            }
        }

        public async Task StreamAsync(CancellationToken token)
        {
            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (Client.IsStreaming)
                throw new InvalidOperationException($"{nameof(BinanceWebSocketStream)}: Already streaming ({nameof(IWebSocketClient)}.{nameof(IWebSocketClient.StreamAsync)} Task is not completed).");

            try
            {
                _bufferBlock = new BufferBlock<string>(new DataflowBlockOptions
                {
                    EnsureOrdered = true,
                    CancellationToken = token,
                    BoundedCapacity = DataflowBlockOptions.Unbounded,
                    MaxMessagesPerTask = DataflowBlockOptions.Unbounded
                });

                _actionBlock = new ActionBlock<string>(json =>
                {
                    try
                    {
                        if (IsCombined)
                        {
                            var jObject = JObject.Parse(json);

                            var stream = jObject["stream"].Value<string>();

                            if (!_subscribers.ContainsKey(stream))
                            {
                                _logger?.LogDebug($"{nameof(BinanceWebSocketStream)}: No subscriber exists for stream: \"{stream}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                                return; // ignore.
                            }

                            var args = new WebSocketStreamEventArgs(stream, jObject["data"].ToString(Formatting.None), token);

                            foreach (var callback in _subscribers[stream])
                            {
                                callback(args);
                            }
                        }
                        else
                        {
                            var args = new WebSocketStreamEventArgs(_subscribers.Keys.Single(), json, token);

                            foreach (var callback in _subscribers.Values.Single())
                            {
                                callback(args);
                            }
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            _logger?.LogError(e, $"{nameof(BinanceWebSocketStream)}: Unhandled {nameof(StreamAsync)} exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        }
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 1,
                    EnsureOrdered = true,
                    //MaxMessagesPerTask = 1,
                    MaxDegreeOfParallelism = 1,
                    CancellationToken = token,
                    SingleProducerConstrained = true
                });

                _bufferBlock.LinkTo(_actionBlock);

                var uri = IsCombined
                    ? new Uri($"{BaseUri}/stream?streams={string.Join("/", _subscribers.Keys)}")
                    : new Uri($"{BaseUri}/ws/{_subscribers.Keys.Single()}");

                Client.Message += OnClientMessage;

                _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(StreamAsync)}: \"{uri.AbsoluteUri}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                await Client.StreamAsync(uri, token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    _logger?.LogError(e, $"{nameof(BinanceWebSocketStream)}.{nameof(StreamAsync)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    throw;
                }
            }
            finally
            {
                Client.Message -= OnClientMessage;

                _bufferBlock?.Complete();
                _actionBlock?.Complete();

                _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(StreamAsync)}: Task complete.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void OnClientMessage(object sender, WebSocketClientEventArgs e)
        {
            // Provides buffering and single-threaded execution.
            _bufferBlock.Post(e.Message);

            var count = _bufferBlock.Count;
            if (count <= _maxBufferCount)
                return;

            _maxBufferCount = count;
            if (_maxBufferCount > 1)
            {
                _logger?.LogTrace($"{nameof(BinanceWebSocketStream)} - Maximum buffer block count: {_maxBufferCount}");
            }
        }

        #endregion Private Methods
    }
}
