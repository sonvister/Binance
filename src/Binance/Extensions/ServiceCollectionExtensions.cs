using Binance.Accounts.Cache;
using Binance.Api;
using Binance.Api.Json;
using Binance.Api.WebSocket;
using Binance.Orders.Book.Cache;
using Binance.Trades.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace Binance
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBinance(this IServiceCollection services)
        {
            // Accounts
            services.AddTransient<IAccountCache, AccountCache>();

            // API
            services.AddSingleton<IBinanceJsonApi, BinanceJsonApi>();
            services.AddTransient<IRateLimiter, RateLimiter>();
            services.AddSingleton<IBinanceApi, BinanceApi>();

            // Orders
            services.AddTransient<IOrderBookCache, OrderBookCache>();

            // Trades
            services.AddTransient<IAggregateTradesCache, AggregateTradesCache>();

            // WebSockets
            services.AddTransient<IDepthWebSocketClient, DepthWebSocketClient>();
            services.AddTransient<IKlineWebSocketClient, KlineWebSocketClient>();
            services.AddTransient<ITradesWebSocketClient, TradesWebSocketClient>();
            services.AddTransient<IUserDataWebSocketClient, UserDataWebSocketClient>();

            return services;
        }
    }
}
