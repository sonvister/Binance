using System;
using System.Threading.Tasks;
using Binance;
using Binance.Api;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp.Examples
{
    internal class MinimalWithDependencyInjection
    {
        public static async Task ExampleMain(string[] args)
        {
            var services = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            using (var api = services.GetService<IBinanceApi>())
            {
                if (await api.PingAsync())
                    Console.WriteLine("SUCCESSFUL!");

                Console.WriteLine("...press any key to continue.");
                Console.ReadKey(true);
            }
        }
    }
}
