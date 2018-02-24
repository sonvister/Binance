using System;
using System.Linq;
using Binance.Api;
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

        public override IUserDataClient Subscribe<TEventArgs>(string listenKey, IBinanceApiUser user, Action<TEventArgs> callback)
        {
            Throw.IfNull(user, nameof(user));

            // Ensure only one user is subscribed.
            if (Users.Count > 0 && !Users.Single().Value.Equals(user))
                throw new InvalidOperationException($"{nameof(SingleUserDataClient)}: Can only subscribe to a single a user.");

            return base.Subscribe(listenKey, user, callback);
        }
    }
}
