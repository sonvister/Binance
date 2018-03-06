using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public interface ITaskController : IError, IDisposable
    {
        /// <summary>
        /// Get the flag indicating if this controller is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Get the controller task.
        /// </summary>
        Task Task { get; }

        /// <summary>
        /// Initiate the controller action.
        /// </summary>
        /// <param name="action">optional.</param>
        void Begin(Func<CancellationToken, Task> action = null);

        /// <summary>
        /// Abort the controller action.
        /// </summary>
        void Abort();

        /// <summary>
        /// Abort and wait for <see cref="Task"/> to complete.
        /// </summary>
        /// <returns></returns>
        Task CancelAsync();

        /// <summary>
        /// Restart the controller.
        /// </summary>
        /// <returns></returns>
        Task RestartAsync();
    }
}
