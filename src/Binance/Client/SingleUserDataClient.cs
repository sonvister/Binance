using System;
using System.Linq;
using Binance.Api;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

namespace Binance.Client
{
    /// <summary>
    /// The default <see cref="ISingleUserDataClient"/> implementation.
    /// </summary>
    public class SingleUserDataClient : UserDataClient, ISingleUserDataClient
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public SingleUserDataClient(ILogger<SingleUserDataClient> logger = null)
            : base(logger)
        { }

        public override void Subscribe(string listenKey, IBinanceApiUser user, Action<UserDataEventArgs> callback)
        {
            Throw.IfNull(user, nameof(user));

            // Ensure only one user is subscribed.
            if (ListenKeys.Count > 0 && !ListenKeys.Single().Value.Equals(user))
                throw new InvalidOperationException($"{nameof(SingleUserDataClient)}: Can only subscribe to a single a user.");

            base.Subscribe(listenKey, user, callback);
        }
    }
}
