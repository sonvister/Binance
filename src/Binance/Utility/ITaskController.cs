using System;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public interface ITaskController : IDisposable
    {
        /// <summary>
        /// Get the flag indicating if this controller is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Get the controller <see cref="Task"/>.
        /// </summary>
        Task Task { get; }

        /// <summary>
        /// Initiate the controller action.
        /// </summary>
        void Begin();

        /// <summary>
        /// Cancel the controller <see cref="Task"/>.
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
