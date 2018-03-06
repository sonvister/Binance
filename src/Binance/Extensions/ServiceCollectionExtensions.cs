using System;
using Binance.Cache;
using Binance.Client;
using Binance.Producer;
using Binance.Serialization;
using Binance.Utility;
using Binance.WebSocket;
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

            // WebSocket
            services.AddTransient<IWatchdogTimer, WatchdogTimer>();
            services.AddSingleton<IClientWebSocketFactory, ClientWebSocketFactory>();
            services.AddTransient<IWebSocketClient, DefaultWebSocketClient>();
            services.AddTransient<ITradeWebSocketClient, TradeWebSocketClient>();
            services.AddTransient<IDepthWebSocketClient, DepthWebSocketClient>();
            services.AddTransient<ICandlestickWebSocketClient, CandlestickWebSocketClient>();
            services.AddTransient<IAggregateTradeWebSocketClient, AggregateTradeWebSocketClient>();
            services.AddTransient<ISymbolStatisticsWebSocketClient, SymbolStatisticsWebSocketClient>();
            services.AddTransient<IUserDataWebSocketClient, UserDataWebSocketClient>();

            services.AddTransient<IAggregateTradeWebSocketCache, AggregateTradeWebSocketCache>();
            services.AddTransient<ICandlestickWebSocketCache, CandlestickWebSocketCache>();
            services.AddTransient<IDepthWebSocketCache, DepthWebSocketCache>();
            services.AddTransient<ISymbolStatisticsWebSocketCache, SymbolStatisticsWebSocketCache>();
            services.AddTransient<ITradeWebSocketCache, TradeWebSocketCache>();

            services.AddTransient<IJsonStream, BinanceWebSocketStream>();
            services.AddTransient<IWebSocketStream, BinanceWebSocketStream>();
            services.AddTransient<IWebSocketStreamController, WebSocketStreamController>();

            if (useSingleCombinedStream)
            {
                //services.AddSingleton<IJsonStream>((s) => s.GetService<IBinanceWebSocketStream>());
                //services.AddSingleton<IWebSocketStream>((s) => s.GetService<IBinanceWebSocketStream>());
                services.AddSingleton<IBinanceWebSocketStream, BinanceWebSocketStream>();
                services.AddSingleton<IBinanceWebSocketStreamController, BinanceWebSocketStreamController>();
                services.AddSingleton<IBinanceWebSocketStreamPublisher, BinanceWebSocketStreamPublisher>();
            }
            else
            {
                services.AddTransient<IBinanceWebSocketStream, BinanceWebSocketStream>();
                services.AddTransient<IBinanceWebSocketStreamController, BinanceWebSocketStreamController>();
                services.AddTransient<IBinanceWebSocketStreamPublisher, BinanceWebSocketStreamPublisher>();
            }

            // Manager
            services.AddSingleton<IBinanceJsonClientManager, BinanceWebSocketClientManager>();
            services.AddSingleton<IBinanceWebSocketClientManager, BinanceWebSocketClientManager>();
            services.AddSingleton<IUserDataWebSocketStreamControl, UserDataWebSocketStreamControl>();
            services.AddSingleton<IUserDataWebSocketManager, UserDataWebSocketManager>();

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
