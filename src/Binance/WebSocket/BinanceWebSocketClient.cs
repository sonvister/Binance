using System;
using System.Collections.Generic;
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

        #region Protected Methods

        protected abstract void OnWebSocketEvent(WebSocketStreamEventArgs args, IEnumerable<Action<TEventArgs>> callbacks);

        private void WebSocketCallback(WebSocketStreamEventArgs args)
        {
            OnWebSocketEvent(args, Subscribers.ContainsKey(args.StreamName) ? Subscribers[args.StreamName] : null);
        }

        protected void SubscribeTo(string stream, Action<TEventArgs> callback)
        {
            WebSocket.Subscribe(stream, WebSocketCallback);

            if (callback == null)
                return;

            if (!Subscribers.ContainsKey(stream))
            {
                Subscribers[stream] = new List<Action<TEventArgs>>();
            }

            if (!Subscribers[stream].Contains(callback))
            {
                Subscribers[stream].Add(callback);
            }
        }

        #endregion Protected Methods
    }
}
