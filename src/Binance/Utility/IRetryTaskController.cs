namespace Binance.Utility
{
    public interface IRetryTaskController : ITaskController
    {
        /// <summary>
        /// Get or set the retry delay (milliseconds).
        /// </summary>
        int RetryDelayMilliseconds { get; set; }
    }
}
