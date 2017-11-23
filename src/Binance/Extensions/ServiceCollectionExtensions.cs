using Binance.Api;
using Binance.Api.Json;
using Binance.Api.WebSocket;
using Binance.Cache;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBinance(this IServiceCollection services)
        {
            // Accounts
            services.AddTransient<IAccountInfoCache, AccountInfoCache>();

            // API
            services.AddSingleton<IBinanceApiUserProvider, BinanceApiUserProvider>();
            services.AddSingleton<IBinanceHttpClient, BinanceHttpClient>();
            services.AddSingleton<IBinanceJsonApi, BinanceJsonApi>();
            services.AddTransient<IApiRateLimiter, ApiRateLimiter>();
            services.AddTransient<IRateLimiter, RateLimiter>();
            services.AddSingleton<IBinanceApi, BinanceApi>();

            // WebSocket
            services.AddTransient<IWebSocketClient, WebSocketClient>();

            // Candlesticks
            services.AddTransient<ICandlesticksCache, CandlesticksCache>();

            // Orders
            services.AddTransient<IOrderBookCache, OrderBookCache>();

            // Trades
            services.AddTransient<IAggregateTradesCache, AggregateTradesCache>();

            // WebSockets
            services.AddTransient<IDepthWebSocketClient, DepthWebSocketClient>();
            services.AddTransient<ICandlestickWebSocketClient, CandlestickWebSocketClient>();
            services.AddTransient<ITradesWebSocketClient, TradesWebSocketClient>();
            services.AddTransient<IUserDataWebSocketClient, UserDataWebSocketClient>();

            return services;
        }
    }
}
