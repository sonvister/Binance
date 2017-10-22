using Binance.Api;
using Binance.Api.Json;
using Binance.Api.WebSocket;
using Binance.Orders.Book.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace Binance
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBinance(this IServiceCollection services)
        {
            // Accounts

            // API
            services.AddSingleton<IBinanceJsonApi, BinanceJsonApi>();
            services.AddTransient<IRateLimiter, RateLimiter>();
            services.AddSingleton<IBinanceApi, BinanceApi>();

            // Orders
            services.AddTransient<IOrderBookCache, OrderBookCache>();

            // Trades

            // WebSockets
            services.AddTransient<IDepthWebSocketClient, DepthWebSocketClient>();
            services.AddTransient<IKlineWebSocketClient, KlineWebSocketClient>();
            services.AddTransient<ITradesWebSocketClient, TradesWebSocketClient>();
            services.AddTransient<IUserDataWebSocketClient, UserDataWebSocketClient>();

            return services;
        }
    }
}
