using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.WebSocket
{
    [Collection("Timing Sensitive Tests")]
    public class DefaultWebSocketClientTest
    {
        private readonly Uri _uri;
        private readonly string _subject;
        private const string _message = "{}";
        private readonly DefaultWebSocketClient _webSocket;

        public DefaultWebSocketClientTest()
        {
            _uri = new Uri(BinanceWebSocketStream.BaseUri);
            _subject = _uri.AbsoluteUri;
            _webSocket = CreateWebSocketClient();
        }

        public static DefaultWebSocketClient CreateWebSocketClient()
        {
            var webSocket = new Mock<IClientWebSocket>();
            webSocket.Setup(w => w.State).Returns(WebSocketState.Open);
            webSocket.Setup(w => w.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Returns(Task.Delay(500));
            webSocket.Setup(w => w.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .Returns<ArraySegment<byte>, CancellationToken>((a, t) =>
                {
                    var bytes = Encoding.ASCII.GetBytes(_message);
                    bytes.CopyTo(a.Array, 0);
                    return Task.FromResult(new WebSocketReceiveResult(_message.Length, WebSocketMessageType.Text, true));
                });

            var factory = new Mock<IClientWebSocketFactory>();
            factory.Setup(f => f.CreateClientWebSocket()).Returns(webSocket.Object);

            return new DefaultWebSocketClient(factory.Object);
        }

        [Fact]
        public async Task Throws()
        {
            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("uri", () => _webSocket.StreamAsync(null, cts.Token));
                await Assert.ThrowsAsync<ArgumentException>("token", () => _webSocket.StreamAsync(_uri, CancellationToken.None));
            }
        }

        [Fact]
        public async Task CanceledTask()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();

                await _webSocket.StreamAsync(_uri, cts.Token);
            }
        }

        [Fact]
        public async Task StreamThrows()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                await Assert.ThrowsAsync<ArgumentNullException>("uri", () => _webSocket.StreamAsync(null, cts.Token));

                var task = _webSocket.StreamAsync(_uri, cts.Token);

                await Assert.ThrowsAsync<InvalidOperationException>(() => _webSocket.StreamAsync(_uri, cts.Token));

                await task;
            }
        }

        [Fact]
        public async Task IsOpen()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.False(_webSocket.IsOpen);

                var task = _webSocket.StreamAsync(_uri, cts.Token);

                Assert.False(_webSocket.IsOpen);

                await _webSocket.WaitUntilOpenAsync();

                Assert.True(_webSocket.IsOpen);

                await task;

                Assert.False(_webSocket.IsOpen);
            }
        }

        [Fact]
        public async Task OpenEvent()
        {
            var isOpenEventReceived = false;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                _webSocket.Open += (s, e) =>
                {
                    isOpenEventReceived = true;
                };

                Assert.False(isOpenEventReceived);

                var task = _webSocket.StreamAsync(_uri, cts.Token);

                Assert.False(isOpenEventReceived);

                await _webSocket.WaitUntilOpenAsync();

                Assert.True(isOpenEventReceived);

                await task;
            }
        }

        [Fact]
        public async Task CloseEvent()
        {
            var isCloseEventReceived = false;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                _webSocket.Close += (s, e) =>
                {
                    isCloseEventReceived = true;
                };

                Assert.False(isCloseEventReceived);

                var task = _webSocket.StreamAsync(_uri, cts.Token);

                Assert.False(isCloseEventReceived);

                await _webSocket.WaitUntilOpenAsync();

                Assert.False(isCloseEventReceived);

                await task;

                Assert.True(isCloseEventReceived);
            }
        }

        [Fact]
        public async Task MessageEvent()
        {
            var isMessageEventReceived = false;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                _webSocket.Message += (s, e) =>
                {
                    isMessageEventReceived = e.Subject == _subject && e.Json == _message;
                };

                Assert.False(isMessageEventReceived);

                await _webSocket.StreamAsync(_uri, cts.Token);

                Assert.True(isMessageEventReceived);
            }
        }

        [Fact]
        public async Task WainUntilOpen()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.False(_webSocket.IsOpen);

                var task = _webSocket.StreamAsync(_uri, cts.Token);

                Assert.False(_webSocket.IsOpen);

                // Wait when web socket is not open.
                await _webSocket.WaitUntilOpenAsync();

                Assert.True(_webSocket.IsOpen);

                // Wait when web socket is open.
                await _webSocket.WaitUntilOpenAsync();

                Assert.False(cts.IsCancellationRequested);

                await task;

                Assert.True(cts.IsCancellationRequested);
            }
        }
    }
}
