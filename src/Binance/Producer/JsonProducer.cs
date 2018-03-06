using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Binance.Producer
{
    /// <summary>
    /// An abstract <see cref="IJsonProducer"/> implementation.
    /// </summary>
    public abstract class JsonProducer : IJsonProducer
    {
        #region Public Events

        public event EventHandler<JsonMessageEventArgs> Message
        {
            add
            {
                if (_message == null || !_message.GetInvocationList().Contains(value))
                {
                    _message += value;
                }
            }
            remove => _message -= value;
        }
        private EventHandler<JsonMessageEventArgs> _message;

        #endregion Public Events

        #region Protected Fields

        protected readonly ILogger<JsonProducer> Logger;

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Optional <see cref="ILogger{JsonProvider}"/>.</param>
        protected JsonProducer(ILogger<JsonProducer> logger = null)
        {
            Logger = logger;
        }

        #endregion Constructors

        #region Protected Methods

        /// <summary>
        /// Raise a JSON message event.
        /// </summary>
        /// <param name="json">The JSON message (required).</param>
        /// <param name="subject">The JSON message context (optional).</param>
        protected void OnMessage(string json, string subject = null)
            => OnMessage(new JsonMessageEventArgs(json, subject));

        /// <summary>
        /// Raise a JSON message event.
        /// </summary>
        /// <param name="args">The JSON message event args (required).</param>
        protected void OnMessage(JsonMessageEventArgs args)
        {
            try { _message?.Invoke(this, args); }
            catch (Exception e)
            {
                Logger?.LogWarning(e, $"{GetType().Name}: Unhandled {nameof(Message)} event handler exception.");
            }
        }

        #endregion Protected Methods
    }
}
