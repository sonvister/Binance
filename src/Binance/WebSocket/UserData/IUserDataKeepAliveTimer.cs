using System;
using System.Collections.Generic;
using Binance.Api;

namespace Binance.WebSocket.UserData
{
    public interface IUserDataKeepAliveTimer : IDisposable
    {
        TimeSpan Period { get; set; }

        IEnumerable<IBinanceApiUser> Users { get; }

        void Add(IBinanceApiUser user, string listenKey);

        void Remove(IBinanceApiUser user);

        void RemoveAll();
    }
}
