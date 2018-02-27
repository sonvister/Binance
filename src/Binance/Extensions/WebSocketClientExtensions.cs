using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class WebSocketClientExtensions
    {
        /// <summary>
        /// Wait until web socket is open (connected).
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task WaitUntilOpenAsync(this IWebSocketClient webSocket, CancellationToken token = default)
        {
            Throw.IfNull(webSocket, nameof(webSocket));

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            EventHandler<EventArgs> handler = (s, e) => tcs.SetResult(true);

            webSocket.Open += handler;

            try
            {
                if (token.IsCancellationRequested || webSocket.IsOpen)
                    return;

                if (token.CanBeCanceled)
                {
                    using (token.Register(() => tcs.SetCanceled()))
                    {
                        await tcs.Task.ConfigureAwait(false);
                    }
                }
                else
                {
                    await tcs.Task.ConfigureAwait(false);
                }
            }
            finally { webSocket.Open -= handler; }
        }
    }
}
