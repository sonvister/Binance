namespace Binance
{
    /// <summary>
    /// JSON message event arguments.
    /// </summary>
    public sealed class JsonMessageEventArgs
    {
        /// <summary>
        /// Get the event message subject (can be null).
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// Get the event message (JSON object or array).
        /// </summary>
        public string Json { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="json">The JSON message (required).</param>
        /// <param name="subject">The JSON message context (optional).</param>
        public JsonMessageEventArgs(string json, string subject = null)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            Json = json;
            Subject = subject;
        }
    }
}
