using System;
using System.Threading.Tasks;
using Binance.Api;

namespace BinanceClassicConsoleApp
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var api = new BinanceApi();

                if (await api.PingAsync())
                    Console.WriteLine("SUCCESSFUL!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("...press any key to continue.");
            Console.ReadKey(true);
        }
    }
}
