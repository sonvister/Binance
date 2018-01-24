using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Binance.Application.Logging
{
    public sealed class FileLogger : ILogger
    {
        #region Private Constants

        private const string Spaces = "      ";

        #endregion Private Constants

        #region Private Fields

        private readonly string _filePath;

        private readonly LogLevel _level;

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        public FileLogger(string filePath, LogLevel level)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            _filePath = filePath;
            _level = level;
        }

        #endregion Constructors

        #region Public Methods

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.None)
                return false;

            return logLevel >= _level;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message))
                return;

            try
            {
                lock (_sync)
                {
                    using (var stream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                    using (var streamWriter = new StreamWriter(stream) { AutoFlush = false })
                    {
                        var now = DateTimeOffset.Now;

                        streamWriter.WriteLine($"[{ConvertLogLevelToString(logLevel)}] {now}");

                        foreach (var line in message.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                        {
                            streamWriter.WriteLine($"{Spaces}{line}");
                        }

                        if (exception != null)
                        {
                            streamWriter.WriteLine($"{Spaces}(exception: \"{exception.Message}\")");
                        }

                        streamWriter.Flush();
                    }
                }
            }
            catch { /* ignore */ }
        }

        #endregion Public Methods

        #region Private Methods

        private static string ConvertLogLevelToString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                case LogLevel.None:
                    return "none";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        #endregion

        #region Private Classes

        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();

            private NullScope() { }

            public void Dispose() { }
        }

        #endregion
    }
}
