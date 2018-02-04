using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        #region Public Events

        public event EventHandler<EventArgs> Open
        {
            add { WebSocket.Client.Open += value; }
            remove { WebSocket.Client.Open -= value; }
        }

        public event EventHandler<EventArgs> Close
        {
            add { WebSocket.Client.Close += value; }
            remove { WebSocket.Client.Close -= value; }
        }

        #endregion Public Events

        #region Public Properties

        public IWebSocketStream WebSocket { get; }

        #endregion Public Properties

        #region Protected Fields

        protected readonly ILogger Logger;

        protected readonly IDictionary<string, IList<Action<TEventArgs>>> Subscribers;

        #endregion Protected Fields

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

            Subscribers = new Dictionary<string, IList<Action<TEventArgs>>>();
        }

        #endregion Constructors

        #region Public Methods

        public void UnsubscribeAll()
        {
            foreach (var stream in Subscribers.Keys)
            {
                Logger?.LogDebug($"{GetType().Name}.{nameof(UnsubscribeAll)}: Unsubscribe stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                WebSocket.Unsubscribe(stream, WebSocketCallback);
            }

            Subscribers.Clear();
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract void OnWebSocketEvent(WebSocketStreamEventArgs args, IEnumerable<Action<TEventArgs>> callbacks);

        private void WebSocketCallback(WebSocketStreamEventArgs args)
        {
            if (!Subscribers.ContainsKey(args.StreamName))
            {
                Logger?.LogDebug($"{nameof(BinanceWebSocketClient<TEventArgs>)}.{nameof(WebSocketCallback)} - Ignoring event for non-subscribed stream: \"{args.StreamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                return; // ignore.
            }

            OnWebSocketEvent(args, Subscribers[args.StreamName]);
        }

        /// <summary>
        /// Subscribe to a stream (with optional callback) if not already.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="callback"></param>
        protected void SubscribeStream(string stream, Action<TEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            if (!Subscribers.ContainsKey(stream))
            {
                Logger?.LogDebug($"{nameof(BinanceWebSocketClient<TEventArgs>)}.{nameof(SubscribeStream)}: Adding stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                Subscribers[stream] = new List<Action<TEventArgs>>();
                WebSocket.Subscribe(stream, WebSocketCallback);
            }

            if (callback != null && !Subscribers[stream].Contains(callback))
            {
                Logger?.LogDebug($"{nameof(BinanceWebSocketClient<TEventArgs>)}.{nameof(SubscribeStream)}: Adding callback for stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                Subscribers[stream].Add(callback);
            }
        }

        /// <summary>
        /// Unsubscribe from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="callback"></param>
        protected void UnsubscribeStream(string stream, Action<TEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            if (callback != null && Subscribers.ContainsKey(stream))
            {
                if (Subscribers[stream].Contains(callback))
                {
                    Logger?.LogDebug($"{nameof(BinanceWebSocketClient<TEventArgs>)}.{nameof(UnsubscribeStream)}: Removing callback for stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    Subscribers[stream].Remove(callback);
                }
            }

            if (callback == null || (Subscribers.ContainsKey(stream) && !Subscribers[stream].Any()))
            {
                WebSocket.Unsubscribe(stream, WebSocketCallback);

                Logger?.LogDebug($"{nameof(BinanceWebSocketClient<TEventArgs>)}.{nameof(UnsubscribeStream)}: Removing stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                Subscribers.Remove(stream);
            }
        }

        #endregion Protected Methods
    }
}
