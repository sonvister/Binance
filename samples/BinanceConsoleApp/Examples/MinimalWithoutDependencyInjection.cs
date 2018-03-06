using System;
using System.Threading.Tasks;
using Binance;

// ReSharper disable once CheckNamespace
namespace BinanceConsoleApp
{
    internal class MinimalWithoutDependencyInjection
    {
        public static async Task ExampleMain(string[] args)
        {
            // Initialize with default services and no logger.
            var api = new BinanceApi();

            if (await api.PingAsync())
                Console.WriteLine("SUCCESSFUL!");

            Console.WriteLine("...press any key to continue.");
            Console.ReadKey(true);
        }
    }
}
