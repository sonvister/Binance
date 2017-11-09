using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Binance.Application
{
    public abstract class TaskServiceController<TService> : IDisposable
        where TService : IDisposable
    {
        #region Private Fields

        private IServiceProvider _services;

        private Task _task;

        private CancellationTokenSource _cts;

        #endregion Private Fields

        #region Constructors

        public TaskServiceController(IServiceProvider services)
        {
            _services = services;
            _cts = new CancellationTokenSource();
        }

        #endregion Constructors

        #region Public Methods

        public void Run(Func<TService, CancellationToken, Task> action)
        {
            _task = Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    using (var service = _services.GetService<TService>())
                    {
                        try { await action(service, _cts.Token); }
                        catch (OperationCanceledException) { }
                        catch (Exception e) { OnError(e); }
                    }

                    if (!_cts.IsCancellationRequested)
                    {
                        await Task.Delay(5000, _cts.Token); // ...wait a bit.
                    }
                }
            });
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnError(Exception e) { }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _cts?.Cancel();
                _task?.GetAwaiter().GetResult();
                _cts?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
