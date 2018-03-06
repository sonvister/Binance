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
    public class DefaultWebSocketClientTest
    {
        private readonly Uri _uri;
        private const string _message = "{}";
        private readonly DefaultWebSocketClient _client;

        public DefaultWebSocketClientTest()
        {
            _uri = new Uri(BinanceWebSocketStream.BaseUri);

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

            _client = new DefaultWebSocketClient(factory.Object);
        }

        [Fact]
        public async Task Throws()
        {
            using (var cts = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("uri", () => _client.StreamAsync(null, cts.Token));
                await Assert.ThrowsAsync<ArgumentException>("token", () => _client.StreamAsync(_uri, CancellationToken.None));
            }
        }

        [Fact]
        public async Task CanceledTask()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();

                await _client.StreamAsync(_uri, cts.Token);
            }
        }

        [Fact]
        public async Task IsStreaming()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.False(_client.IsStreaming);

                var task = _client.StreamAsync(_uri, cts.Token);

                Assert.True(_client.IsStreaming);

                await task;

                Assert.False(_client.IsStreaming);
            }
        }

        [Fact]
        public async Task IsOpen()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.False(_client.IsOpen);

                var task = _client.StreamAsync(_uri, cts.Token);

                Assert.False(_client.IsOpen);

                await _client.WaitUntilOpenAsync();

                Assert.True(_client.IsOpen);

                await task;

                Assert.False(_client.IsOpen);
            }
        }

        [Fact]
        public async Task OpenEvent()
        {
            var isOpenEventReceived = false;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                _client.Open += (s, e) =>
                {
                    isOpenEventReceived = true;
                };

                Assert.False(isOpenEventReceived);

                var task = _client.StreamAsync(_uri, cts.Token);

                Assert.False(isOpenEventReceived);

                await _client.WaitUntilOpenAsync();

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
                _client.Close += (s, e) =>
                {
                    isCloseEventReceived = true;
                };

                Assert.False(isCloseEventReceived);

                var task = _client.StreamAsync(_uri, cts.Token);

                Assert.False(isCloseEventReceived);

                await _client.WaitUntilOpenAsync();

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
                _client.Message += (s, e) =>
                {
                    isMessageEventReceived = e.Subject == _uri.AbsoluteUri && e.Json == _message;
                };

                Assert.False(isMessageEventReceived);

                await _client.StreamAsync(_uri, cts.Token);

                Assert.True(isMessageEventReceived);
            }
        }

        [Fact]
        public async Task WainUntilOpen()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.False(_client.IsOpen);

                var task = _client.StreamAsync(_uri, cts.Token);

                Assert.False(_client.IsOpen);

                // Wait when web socket is not open.
                await _client.WaitUntilOpenAsync();

                Assert.True(_client.IsOpen);

                // Wait when web socket is open.
                await _client.WaitUntilOpenAsync();

                Assert.False(cts.IsCancellationRequested);

                await task;

                Assert.True(cts.IsCancellationRequested);
            }
        }
    }
}
