using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// Abstract Binance web socket client base class.
    /// </summary>
    public abstract class BinanceWebSocketClient<TEventArgs> : IBinanceWebSocketClient
        where TEventArgs : EventArgs
    {
        #region Public Properties

        public IWebSocketStream WebSocket { get; }

        #endregion Public Properties

        #region Protected Fields

        protected BufferBlock<string> BufferBlock;
        protected ActionBlock<string> ActionBlock;

        protected readonly ILogger Logger;

        protected readonly IDictionary<string, IList<Action<TEventArgs>>> _subscribers;

        #endregion Protected Fields

        #region Private Fields

        private int _maxBufferCount;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="logger"></param>
        protected BinanceWebSocketClient(IWebSocketStream webSocket, ILogger logger = null)
        {
            Throw.IfNull(webSocket, nameof(webSocket));

            WebSocket = webSocket;
            Logger = logger;

            _subscribers = new Dictionary<string, IList<Action<TEventArgs>>>();
        }

        #endregion Constructors

        #region Protected Methods

        protected abstract void OnWebSocketEvent(WebSocketStreamEventArgs args, IEnumerable<Action<TEventArgs>> callbacks);

        private void WebSocketCallback(WebSocketStreamEventArgs args)
        {
            OnWebSocketEvent(args, _subscribers.ContainsKey(args.StreamName) ? _subscribers[args.StreamName] : null);
        }

        protected void SubscribeTo(string stream, Action<TEventArgs> callback)
        {
            WebSocket.Subscribe(stream, WebSocketCallback);

            if (callback == null)
                return;

            if (!_subscribers.ContainsKey(stream))
            {
                _subscribers[stream] = new List<Action<TEventArgs>>();
            }

            if (!_subscribers[stream].Contains(callback))
            {
                _subscribers[stream].Add(callback);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnClientMessage(object sender, WebSocketClientEventArgs e)
        {
            // Provides buffering and single-threaded execution.
            BufferBlock.Post(e.Message);

            var count = BufferBlock.Count;
            if (count <= _maxBufferCount)
                return;

            _maxBufferCount = count;
            if (_maxBufferCount > 1)
            {
                Logger?.LogTrace($"{GetType().Name} - Maximum buffer block count: {_maxBufferCount}");
            }
        }

        #endregion Private Methods
    }
}
