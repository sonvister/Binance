using Binance;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddBinance();

            return services;
        }
    }
}
