using System;
using Binance.Client;

namespace Binance.Manager
{
    public sealed class ClientManagerErrorEventArgs : ErrorEventArgs
    {
        public IJsonClient Client { get; }

        public ClientManagerErrorEventArgs(IJsonClient client, Exception exception)
            : base(exception)
        {
            Throw.IfNull(client, nameof(client));

            Client = client;
        }
    }
}
