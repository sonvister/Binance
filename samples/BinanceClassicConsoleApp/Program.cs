using System;
using System.Threading.Tasks;
using Binance.Api;

namespace BinanceClassicConsoleApp
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var api = new BinanceApi();

            if (await api.PingAsync())
                Console.WriteLine("SUCCESSFUL!");

            Console.WriteLine("...press any key to continue.");
            Console.ReadKey(true);
        }
    }
}
