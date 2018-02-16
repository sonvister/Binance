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
        private volatile bool _isOpen;

        public WebSocketSharpClient(ILogger<WebSocketSharpClient> logger = null)
            : base(logger)
        { }

        public override async Task StreamAsync(Uri uri, CancellationToken token)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (IsStreaming)
                throw new InvalidOperationException($"{nameof(WebSocketSharpClient)}.{nameof(StreamAsync)}: Already streaming (this method is not reentrant).");

            IsStreaming = true;

            Exception exception = null;

            var tcs = new TaskCompletionSource<object>();
            token.Register(() => tcs.TrySetCanceled());

            var webSocket = new WebSocketSharp.WebSocket(uri.AbsoluteUri);
            webSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            webSocket.Log.Output = (d, s) => { /* ignore */ };

            webSocket.OnOpen += (s, e) =>
            {
                _isOpen = true;
                RaiseOpenEvent();
            };

            webSocket.OnClose += (s, e) => tcs.TrySetCanceled();

            webSocket.OnMessage += (s, evt) =>
            {
                try
                {
                    if (!evt.IsText)
                        return;

                    var json = evt.Data;

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        RaiseMessageEvent(json, uri.AbsolutePath);
                    }
                    else
                    {
                        Logger?.LogWarning($"{nameof(WebSocketSharpClient)}.OnMessage: Received empty JSON message.");
                    }
                }
                //catch (OperationCanceledException) { /* ignored */ }
                catch (Exception e)
                {
                    if (!token.IsCancellationRequested)
                    {
                        Logger?.LogError(e, $"{nameof(WebSocketSharpClient)}.OnMessage: WebSocket read exception.");
                        exception = e;
                        tcs.TrySetCanceled();
                    }
                }
            };

            webSocket.OnError += (s, e) =>
            {
                if (token.IsCancellationRequested)
                    return;

                Logger?.LogError(e.Exception, $"{nameof(WebSocketSharpClient)}.OnError: WebSocket exception.");
                exception = e.Exception;
                tcs.TrySetCanceled();
            };

            try
            {
                webSocket.Connect();

                await tcs.Task
                    .ConfigureAwait(false);

                if (exception != null)
                    throw exception;
            }
            //catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(WebSocketSharpClient)}.{nameof(StreamAsync)}: WebSocket connect exception.");
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
                        Logger?.LogError(e, $"{nameof(WebSocketSharpClient)}.{nameof(StreamAsync)}: WebSocket close exception.");
                    }
                }

                ((IDisposable) webSocket).Dispose();

                if (_isOpen)
                {
                    _isOpen = false;
                    RaiseCloseEvent();
                }

                IsStreaming = false;
            }
        }
    }
}
