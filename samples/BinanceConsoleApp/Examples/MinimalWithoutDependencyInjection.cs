using System;
using System.Threading.Tasks;
using Binance.Api;

namespace BinanceConsoleApp.Examples
{
    internal class MinimalWithoutDependencyInjection
    {
        public static async Task ExampleMain(string[] args)
        {
            using (var api = new BinanceApi())
            {
                if (await api.PingAsync())
                    Console.WriteLine("SUCCESSFUL!");

                Console.WriteLine("...press any key to continue.");
                Console.ReadKey(true);
            }
        }
    }
}
