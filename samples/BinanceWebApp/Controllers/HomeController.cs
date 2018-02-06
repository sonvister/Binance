using System.Diagnostics;
using System.Threading.Tasks;
using Binance.Api;
using BinanceWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BinanceWebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var api = new BinanceApi();

            var time = await api.GetTimeAsync();

            return Content($"Server Time: {time} UTC");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
