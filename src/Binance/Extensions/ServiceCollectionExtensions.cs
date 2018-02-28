using System;
using Binance.Api;
using Binance.Cache;
using Binance.Client;
using Binance.Manager;
using Binance.Serialization;
using Binance.Stream;
using Binance.Utility;
using Binance.WebSocket;
using Binance.WebSocket.Manager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBinance(this IServiceCollection services, bool useSingleCombinedStream = false)
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

            // Cache
            services.AddTransient<IAccountInfoCache, AccountInfoCache>();
            services.AddTransient<IAggregateTradeCache, AggregateTradeCache>();
            services.AddTransient<ITradeCache, TradeCache>();
            services.AddTransient<IOrderBookCache, OrderBookCache>();
            services.AddTransient<ICandlestickCache, CandlestickCache>();
            services.AddTransient<ISymbolStatisticsCache, SymbolStatisticsCache>();

            // Client
            services.AddTransient<IAggregateTradeClient, AggregateTradeClient>();
            services.AddTransient<ICandlestickClient, CandlestickClient>();
            services.AddTransient<IDepthClient, DepthClient>();
            services.AddTransient<ISymbolStatisticsClient, SymbolStatisticsClient>();
            services.AddTransient<ITradeClient, TradeClient>();
            services.AddTransient<IUserDataClient, UserDataClient>();

            // Manager
            services.AddTransient<IAggregateTradeClientManager, AggregateTradeClientManager>();
            services.AddTransient<ICandlestickClientManager, CandlestickClientManager>();
            services.AddTransient<IDepthClientManager, DepthClientManager>();
            services.AddTransient<ISymbolStatisticsClientManager, SymbolStatisticsClientManager>();
            services.AddTransient<ITradeClientManager, TradeClientManager>();
            services.AddTransient<IUserDataClientManager, UserDataClientManager>();

            // WebSocket
            services.AddTransient<IWatchdogTimer, WatchdogTimer>();
            services.AddTransient<IWebSocketClient, DefaultWebSocketClient>();
            services.AddTransient<ITradeWebSocketClient, TradeWebSocketClient>();
            services.AddTransient<IDepthWebSocketClient, DepthWebSocketClient>();
            services.AddTransient<ICandlestickWebSocketClient, CandlestickWebSocketClient>();
            services.AddTransient<IAggregateTradeWebSocketClient, AggregateTradeWebSocketClient>();
            services.AddTransient<ISymbolStatisticsWebSocketClient, SymbolStatisticsWebSocketClient>();
            services.AddTransient<IUserDataWebSocketClient, UserDataWebSocketClient>();

            if (useSingleCombinedStream)
            {
                // NOTE: Each of these interfaces have a distinct JSON stream singleton.
                services.AddSingleton<IJsonStream, BinanceWebSocketStream>();
                services.AddSingleton<IWebSocketStream, BinanceWebSocketStream>();
                services.AddSingleton<IBinanceWebSocketStream, BinanceWebSocketStream>();

                // NOTE: Each of these interfaces have a distinct JSON stream controller singleton.
                services.AddSingleton<IJsonStreamController, JsonStreamController>();
                services.AddSingleton<IWebSocketStreamController, BinanceWebSocketStreamController>();
            }
            else
            {
                services.AddTransient<IJsonStream, BinanceWebSocketStream>();
                services.AddTransient<IWebSocketStream, BinanceWebSocketStream>();
                services.AddTransient<IBinanceWebSocketStream, BinanceWebSocketStream>();

                services.AddTransient<IJsonStreamController, JsonStreamController>();
                services.AddTransient<IWebSocketStreamController, BinanceWebSocketStreamController>();
            }

            services.AddTransient<IBinanceJsonClientManager, BinanceWebSocketClientManager>();
            services.AddTransient<IBinanceWebSocketClientManager, BinanceWebSocketClientManager>();
            services.AddTransient<IAggregateTradeWebSocketClientManager, AggregateTradeWebSocketClientManager>();
            services.AddTransient<ICandlestickWebSocketClientManager, CandlestickWebSocketClientManager>();
            services.AddTransient<IDepthWebSocketClientManager, DepthWebSocketClientManager>();
            services.AddTransient<ISymbolStatisticsWebSocketClientManager, SymbolStatisticsWebSocketClientManager>();
            services.AddTransient<ITradeWebSocketClientManager, TradeWebSocketClientManager>();
            services.AddTransient<IUserDataWebSocketManager, UserDataWebSocketClientManager>();
            services.AddSingleton<IUserDataWebSocketStreamControl, UserDataWebSocketStreamControl>();

            services.AddTransient<IAggregateTradeWebSocketCacheManager, AggregateTradeWebSocketCacheManager>();
            services.AddTransient<ICandlestickWebSocketCacheManager, CandlestickWebSocketCacheManager>();
            services.AddTransient<IDepthWebSocketCacheManager, DepthWebSocketCacheManager>();
            services.AddTransient<ISymbolStatisticsWebSocketCacheManager, SymbolStatisticsWebSocketCacheManager>();
            services.AddTransient<ITradeWebSocketCacheManager, TradeWebSocketCacheManager>();

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
