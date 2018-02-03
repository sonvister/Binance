using System;
using System.Linq;
using Binance.Api;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.UserData
{
    public class SingleUserDataWebSocketClient : UserDataWebSocketClient, ISingleUserDataWebSocketClient
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SingleUserDataWebSocketClient()
            : this (new BinanceApi(), new WebSocketStreamProvider())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="streamProvider"></param>
        /// <param name="logger"></param>
        public SingleUserDataWebSocketClient(IBinanceApi api, IWebSocketStreamProvider streamProvider, ILogger<SingleUserDataWebSocketClient> logger = null)
            : base(api, streamProvider.CreateStream(), logger)
        { }

        public override void Subscribe(string listenKey, IBinanceApiUser user, Action<UserDataEventArgs> callback)
        {
            Throw.IfNull(user, nameof(user));

            if (ListenKeys.Count > 0 && ListenKeys.Single().Value != user)
                throw new InvalidOperationException($"{nameof(SingleUserDataWebSocketClient)}: Can only subscribe to a single a user.");

            base.Subscribe(listenKey, user, callback);
        }
    }
}
