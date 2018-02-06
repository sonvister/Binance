using System;
using Microsoft.Extensions.DependencyInjection;

namespace Binance.WebSocket.UserData
{
    public sealed class UserDataKeepAliveTimerProvider : IUserDataKeepAliveTimerProvider
    {
        private readonly IServiceProvider _services;

        public UserDataKeepAliveTimerProvider()
        { }

        public UserDataKeepAliveTimerProvider(IServiceProvider services)
        {
            Throw.IfNull(services, nameof(services));

            _services = services;
        }

        public IUserDataKeepAliveTimer CreateTimer()
        {
            return _services == null
                ? new UserDataKeepAliveTimer()
                : _services.GetService<IUserDataKeepAliveTimer>();
        }
    }
}
