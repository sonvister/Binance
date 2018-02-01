using System;
using Binance.Api;
using Binance.Cache;
using Binance.Serialization;
using Binance.WebSocket;
using Binance.WebSocket.UserData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBinance(this IServiceCollection services)
        {
            // API
            services.AddSingleton<IBinanceApiUserProvider, BinanceApiUserProvider>();
            services.AddSingleton<ITimestampProvider, TimestampProvider>();
            services.AddSingleton<IBinanceHttpClient>(s =>
            {
                if (!BinanceHttpClient.Initializer.IsValueCreated)
                {
                    // Replace initializer.
                    BinanceHttpClient.Initializer = new Lazy<BinanceHttpClient>(() =>
                        new BinanceHttpClient(
                            s.GetService<ITimestampProvider>(),
                            s.GetService<IApiRateLimiter>(),
                            s.GetService<IOptions<BinanceApiOptions>>(),
                            s.GetService<ILogger<BinanceHttpClient>>()), true);
                }

                return BinanceHttpClient.Instance;
            });
            services.AddTransient<IApiRateLimiter, ApiRateLimiter>();
            services.AddTransient<IRateLimiter, RateLimiter>();
            services.AddSingleton<IBinanceApi, BinanceApi>();

            // WebSocket
            services.AddTransient<IWebSocketClient, DefaultWebSocketClient>();
            services.AddTransient<IWebSocketStream, BinanceWebSocketStream>();
            services.AddTransient<BinanceWebSocketStream, BinanceWebSocketStream>();

            // Cache
            services.AddTransient<ITradeCache, TradeCache>();
            services.AddTransient<IOrderBookCache, OrderBookCache>();
            services.AddTransient<IAccountInfoCache, AccountInfoCache>();
            services.AddTransient<ICandlestickCache, CandlestickCache>();
            services.AddTransient<IAggregateTradeCache, AggregateTradeCache>();
            services.AddTransient<ISymbolStatisticsCache, SymbolStatisticsCache>();

            // WebSockets
            services.AddSingleton<IWebSocketStreamProvider, WebSocketStreamProvider>();
            services.AddTransient<ITradeWebSocketClient, TradeWebSocketClient>();
            services.AddTransient<IDepthWebSocketClient, DepthWebSocketClient>();
            services.AddTransient<ICandlestickWebSocketClient, CandlestickWebSocketClient>();
            services.AddTransient<IAggregateTradeWebSocketClient, AggregateTradeWebSocketClient>();
            services.AddTransient<ISymbolStatisticsWebSocketClient, SymbolStatisticsWebSocketClient>();
            services.AddTransient<IMultiUserDataWebSocketClient, MultiUserDataWebSocketClient>();
            services.AddTransient<IUserDataWebSocketClient, SingleUserDataWebSocketClient>();
            services.AddTransient<IUserDataKeepAliveTimer, UserDataKeepAliveTimer>();
            services.AddTransient<IUserDataKeepAliveTimerProvider, UserDataKeepAliveTimerProvider>();

            // Serialization
            services.AddSingleton<IOrderBookTopSerializer, OrderBookTopSerializer>();
            services.AddSingleton<IOrderBookSerializer, OrderBookSerializer>();
            services.AddSingleton<ICandlestickSerializer, CandlestickSerializer>();
            services.AddSingleton<ISymbolPriceSerializer, SymbolPriceSerializer>();
            services.AddSingleton<ISymbolStatisticsSerializer, SymbolStatisticsSerializer>();
            services.AddSingleton<IAggregateTradeSerializer, AggregateTradeSerializer>();
            services.AddSingleton<IAccountTradeSerializer, AccountTradeSerializer>();
            services.AddSingleton<ITradeSerializer, TradeSerializer>();
            services.AddSingleton<IOrderSerializer, OrderSerializer>();

            return services;
        }
    }
}
