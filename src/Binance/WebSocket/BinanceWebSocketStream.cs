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
            add => Client.Open += value;
            remove => Client.Open -= value;
        }

        public event EventHandler<EventArgs> Close
        {
            add => Client.Close += value;
            remove => Client.Close -= value;
        }

        #endregion Public Events

        #region Public Properties

        public IWebSocketClient Client { get; }

        public IEnumerable<string> SubscribedStreams
        {
            get
            {
                // TODO
                //lock (_sync)
                //{
                    return _subscribers.Keys.ToArray();
                //}
            }
        }

        public bool IsCombined
        {
            get
            {
                // TODO
                //lock (_sync)
                //{
                    return _subscribers.Count > 1;
                //}
            }
        }

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

        //private readonly object _sync = new object(); // TODO

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

            // TODO
            //lock (_sync)
            //{
                if (!_subscribers.ContainsKey(stream))
                {
                    if (Client.IsStreaming)
                    {
                        _logger?.LogError($"{nameof(BinanceWebSocketStream)}.{nameof(Subscribe)}: {nameof(IWebSocketClient)} is streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        throw new InvalidOperationException($"{nameof(BinanceWebSocketStream)}.{nameof(Subscribe)}: Subscribing (a stream) must be done when not streaming.");
                    }

                    _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(Subscribe)}: Adding stream: \"{stream}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _subscribers[stream] = new List<Action<WebSocketStreamEventArgs>>();
                }

                // ReSharper disable once InvertIf
                if (!_subscribers[stream].Contains(callback))
                {
                    _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(Subscribe)}: Adding callback for stream: \"{stream}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _subscribers[stream].Add(callback);
                }
            //}
        }

        public void Unsubscribe(string stream, Action<WebSocketStreamEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            _logger?.LogDebug($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: \"{stream}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            // TODO
            //lock (_sync)
            //{
                if (!_subscribers.ContainsKey(stream))
                {
                    _logger?.LogError($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: Not subscribed to stream: \"{stream}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    throw new InvalidOperationException($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: Not subscribed to stream: \"{stream}\"");
                }

                if (_subscribers[stream].Contains(callback))
                {
                    _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: Removing callback for stream: \"{stream}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _subscribers[stream].Remove(callback);
                }

                // Unsubscribe stream if there are no callbacks.
                // ReSharper disable once InvertIf
                if (!_subscribers[stream].Any())
                {
                    if (Client.IsStreaming)
                    {
                        _logger?.LogError($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: {nameof(IWebSocketClient)} is streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        throw new InvalidOperationException($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: Unsubscribing (a stream) must be done when not streaming.");
                    }

                    _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(Unsubscribe)}: Removing stream: \"{stream}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _subscribers.Remove(stream);
                }
            //}
        }

        public void UnsubscribeAll()
        {
            // TODO
            //lock (_sync)
            //{
                _logger?.LogDebug($"{nameof(BinanceWebSocketStream)}.{nameof(UnsubscribeAll)}: Removing all streams.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                _subscribers.Clear();
            //}
        }

        public async Task StreamAsync(CancellationToken token)
        {
            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (Client.IsStreaming)
                throw new InvalidOperationException($"{nameof(BinanceWebSocketStream)}.{nameof(StreamAsync)}: Already streaming ({nameof(IWebSocketClient)}.{nameof(IWebSocketClient.StreamAsync)} Task is not completed).");

            if (!_subscribers.Any())
                throw new InvalidOperationException($"{nameof(BinanceWebSocketStream)}.{nameof(StreamAsync)}: Not subscribed to any streams.");

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
                        string streamName;

                        Action<WebSocketStreamEventArgs>[] callbacks;

                        // TODO: Avoid locking... allowing for eventual consistency of callbacks.
                        //lock (_sync)
                        //{
                            if (_subscribers.Count > 1) // using combined streams.
                            {
                                var jObject = JObject.Parse(json);

                                // Get stream name.
                                streamName = jObject["stream"]?.Value<string>();
                                if (streamName == null)
                                {
                                    _logger?.LogError($"{nameof(BinanceWebSocketStream)}: No 'stream' name in message: \"{json}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                                    return; // ignore.
                                }

                                // Get JSON data.
                                var data = jObject["data"]?.ToString(Formatting.None);
                                if (data == null)
                                {
                                    _logger?.LogError($"{nameof(BinanceWebSocketStream)}: No JSON 'data' in message: \"{json}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                                    return; // ignore.
                                }

                                json = data;
                            }
                            else
                            {
                                // Get stream name.
                                streamName = _subscribers.Keys.FirstOrDefault();
                                if (streamName == null)
                                {
                                    _logger?.LogError($"{nameof(BinanceWebSocketStream)}: No subscribed streams.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                                    return; // ignore.
                                }
                            }

                            if (_subscribers.TryGetValue(streamName, out var subscribers))
                            {
                                // Get subscribed callbacks.
                                callbacks = subscribers?.ToArray();
                            }
                            else
                            {
                                _logger?.LogError($"{nameof(BinanceWebSocketStream)}: No subscribers for stream: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                                return; // ignore.
                            }
                        //}

                        if (callbacks == null)
                            return;

                        // Create event with stream name and JSON data.
                        var args = new WebSocketStreamEventArgs(streamName, json, token);

                        foreach (var callback in callbacks)
                        {
                            callback(args);
                        }
                    }
                    catch (OperationCanceledException) { /* ignored */ }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            _logger?.LogError(e, $"{nameof(BinanceWebSocketStream)}: Failed processing JSON message.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
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
            catch (OperationCanceledException) { /* ignored */ }
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
                _logger?.LogTrace($"{nameof(BinanceWebSocketStream)}.{nameof(OnClientMessage)}: - Maximum buffer block count: {_maxBufferCount}");
            }
        }

        #endregion Private Methods
    }
}
