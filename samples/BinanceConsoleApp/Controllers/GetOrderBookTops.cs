using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetOrderBookTops : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("tops", StringComparison.OrdinalIgnoreCase))
                return false;

            var tops = await Program._api.GetOrderBookTopsAsync(token);

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                foreach (var top in tops)
                {
                    Console.WriteLine($"  {top.Symbol.PadLeft(8)}  -  Bid: {top.Bid.Price.ToString().PadLeft(12)} (qty: {top.Bid.Quantity})  |  Ask: {top.Ask.Price} (qty: {top.Ask.Quantity})");
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
