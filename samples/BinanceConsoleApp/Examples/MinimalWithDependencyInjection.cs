using System;
using System.Threading.Tasks;
using Binance;
using Binance.Api;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace BinanceConsoleApp
{
    internal class MinimalWithDependencyInjection
    {
        public static async Task ExampleMain(string[] args)
        {
            var services = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            var api = services.GetService<IBinanceApi>();

            if (await api.PingAsync())
                Console.WriteLine("SUCCESSFUL!");

            Console.WriteLine("...press any key to continue.");
            Console.ReadKey(true);
        }
    }
}
