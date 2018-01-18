using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Api.WebSocket
{
    public sealed class BinanceWebSocketStream : IWebSocketStream
    {
        #region Public Properties

        public IWebSocketClient WebSocket { get; }

        public bool IsCombined { get; private set; }

        #endregion Public Properties

        #region Private Constants

        private const string BaseUri = "wss://stream.binance.com:9443";

        #endregion Private Constants

        #region Private Fields

        private int _maxBufferCount;

        private BufferBlock<string> _bufferBlock;
        private ActionBlock<string> _actionBlock;

        private readonly ILogger<BinanceWebSocketStream> _logger;

        private readonly IDictionary<string, IList<Action<WebSocketStreamEventArgs>>> _subscribers;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default web socket client, but no logging.
        /// </summary>
        public BinanceWebSocketStream()
            : this(new WebSocketClient(), null)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger">The logger (optional).</param>
        public BinanceWebSocketStream(IWebSocketClient client, ILogger<BinanceWebSocketStream> logger = null)
        {
            Throw.IfNull(client, nameof(client));

            WebSocket = client;
            _logger = logger;

            _subscribers = new Dictionary<string, IList<Action<WebSocketStreamEventArgs>>>();
        }

        #endregion Constructors

        #region Public Properties

        public void Subscribe(string stream, Action<WebSocketStreamEventArgs> callback)
        {
            if (!_subscribers.ContainsKey(stream))
            {
                if (WebSocket.IsStreaming)
                    throw new InvalidOperationException($"{nameof(IWebSocketClient)} is already streaming.");

                _subscribers[stream] = new List<Action<WebSocketStreamEventArgs>>();
            }

            if (!_subscribers[stream].Contains(callback))
            {
                _subscribers[stream].Add(callback);
            }

            IsCombined = _subscribers.Keys.Count > 1;
        }

        public void Unsubscribe(string streamName, Action<WebSocketStreamEventArgs> callback)
        {
            if (!_subscribers.ContainsKey(streamName))
                return;

            if (_subscribers[streamName].Contains(callback))
            {
                _subscribers[streamName].Remove(callback);
            }
        }

        public async Task StreamAsync(CancellationToken token)
        {
            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (WebSocket.IsStreaming)
                throw new InvalidOperationException($"{nameof(BinanceWebSocketStream)}: Already streaming.");

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
                                _logger.LogError($"{nameof(BinanceWebSocketStream)}: No subscriber exists for stream: \"{stream}\"");
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
                            _logger?.LogError(e, $"{nameof(BinanceWebSocketStream)}: Unhandled {nameof(StreamAsync)} exception.");
                        }
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 1,
                    EnsureOrdered = true,
                    MaxDegreeOfParallelism = 1,
                    CancellationToken = token,
                    SingleProducerConstrained = true
                });

                _bufferBlock.LinkTo(_actionBlock);

                var uri = IsCombined
                    ? new Uri($"{BaseUri}/stream?streams={string.Join("/", _subscribers.Keys)}")
                    : new Uri($"{BaseUri}/ws/{_subscribers.Keys.Single()}");

                WebSocket.Message += OnClientMessage;

                _logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(StreamAsync)}: \"{uri.AbsoluteUri}\"");

                await WebSocket.StreamAsync(uri, token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    _logger?.LogError(e, $"{nameof(BinanceWebSocketStream)}.{nameof(StreamAsync)}");
                    throw;
                }
            }
            finally
            {
                WebSocket.Message -= OnClientMessage;

                _bufferBlock?.Complete();
                _actionBlock?.Complete();
            }
        }

        #endregion Public Properties

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
