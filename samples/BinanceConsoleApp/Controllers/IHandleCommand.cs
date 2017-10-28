using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public interface IHandleCommand
    {
        Task<bool> HandleAsync(string command, CancellationToken token = default);
    }
}
