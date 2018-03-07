using System;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebSocketSharp;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public class WebSocketSharpClient : WebSocketClient
    {
        private readonly object _sync = new object();

        public WebSocketSharpClient(ILogger<WebSocketSharpClient> logger = null)
            : base(logger)
        { }

        public override async Task StreamAsync(Uri uri, CancellationToken token)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            if (token.IsCancellationRequested)
                return;

            lock (_sync)
            {
                if (IsStreaming)
                    throw new InvalidOperationException($"{nameof(WebSocketSharpClient)}.{nameof(StreamAsync)}: Already streaming (this method is not reentrant).");

                IsStreaming = true;
            }

            Exception exception = null;

            var tcs = new TaskCompletionSource<object>();

            using (token.Register(() => tcs.TrySetCanceled()))
            {
                var webSocket = new WebSocketSharp.WebSocket(uri.AbsoluteUri);
                webSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                webSocket.Log.Output = (d, s) => { /* ignore */ };

                webSocket.OnOpen += (s, e) =>
                {
                    OnOpen();
                };

                webSocket.OnClose += (s, e) =>
                {
                    if (!tcs.TrySetCanceled())
                    {
                        Logger?.LogWarning($"{nameof(WebSocketSharpClient)}.OnClose: Failed to set task completion source canceled.");
                    }
                };

                webSocket.OnMessage += (s, evt) =>
                {
                    try
                    {
                        if (!evt.IsText)
                            return;

                        var json = evt.Data;

                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            OnMessage(json, uri.AbsolutePath);
                        }
                        else
                        {
                            Logger?.LogWarning($"{nameof(WebSocketSharpClient)}.OnMessage: Received empty JSON message.");
                        }
                    }
                    catch (OperationCanceledException) { /* ignore */ }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Logger?.LogWarning(e, $"{nameof(WebSocketSharpClient)}.OnMessage: Web socket read exception.");
                            exception = e;
                            tcs.TrySetCanceled();
                        }
                    }
                };

                webSocket.OnError += (s, e) =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    Logger?.LogError(e.Exception, $"{nameof(WebSocketSharpClient)}.OnError: Web socket exception.");
                    exception = e.Exception;
                    tcs.TrySetCanceled();
                };

                try
                {
                    Logger?.LogInformation($"{nameof(WebSocketSharpClient)}.{nameof(StreamAsync)}: Web socket connecting...");

                    webSocket.Connect();

                    await tcs.Task
                        .ConfigureAwait(false);

                    if (exception != null)
                        throw exception;
                }
                catch (OperationCanceledException) { /* ignore */ }
                catch (Exception e)
                {
                    if (!token.IsCancellationRequested)
                    {
                        Logger?.LogWarning(e, $"{nameof(WebSocketSharpClient)}.{nameof(StreamAsync)}: Web socket connect exception.");
                        throw;
                    }
                }
                finally
                {
                    if (webSocket.IsAlive)
                    {
                        try { webSocket.Close(CloseStatusCode.Normal); }
                        catch (Exception e)
                        {
                            Logger?.LogWarning(e, $"{nameof(WebSocketSharpClient)}.{nameof(StreamAsync)}: Web socket close exception.");
                        }
                    }

                    ((IDisposable)webSocket).Dispose();

                    lock (_sync) { IsStreaming = false; }

                    OnClose();

                    Logger?.LogDebug($"{nameof(WebSocketSharpClient)}.{nameof(StreamAsync)}: Task complete.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                }
            }
        }
    }
}
