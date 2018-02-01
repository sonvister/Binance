using System;
using Microsoft.Extensions.DependencyInjection;

namespace Binance.WebSocket.UserData
{
    public sealed class UserDataKeepAliveTimerProvider : IUserDataKeepAliveTimerProvider
    {
        private IServiceProvider _services;

        public UserDataKeepAliveTimerProvider()
        { }

        public UserDataKeepAliveTimerProvider(IServiceProvider services)
        {
            Throw.IfNull(services, nameof(services));

            _services = services;
        }

        public IUserDataKeepAliveTimer CreateTimer()
        {
            if (_services == null)
                return new UserDataKeepAliveTimer();

            return _services.GetService<IUserDataKeepAliveTimer>();
        }
    }
}
