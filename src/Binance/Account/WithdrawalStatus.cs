namespace Binance.Account
{
    public enum WithdrawalStatus
    {
        /// <summary>
        /// Email sent.
        /// </summary>
        EmailSent = 0,

        /// <summary>
        /// Cancelled.
        /// </summary>
        Cancelled = 1,

        /// <summary>
        /// Awaiting approval.
        /// </summary>
        AwaitingApproval = 2,

        /// <summary>
        /// Rejected.
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Processing.
        /// </summary>
        Processing = 4,

        /// <summary>
        /// Failure.
        /// </summary>
        Failure = 5,

        /// <summary>
        /// Completed.
        /// </summary>
        Completed = 6
    }
}
