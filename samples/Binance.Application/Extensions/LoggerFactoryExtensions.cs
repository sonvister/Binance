using System;
using Binance.Application.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.Application
{
    public static class LoggerFactoryExtensions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory factory, IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            const string filePathSectionKey = "Path";

            var filePathSection = configuration.GetSection(filePathSectionKey);
            if (filePathSection == null)
                throw new Exception($"File logger configuration does not contain a '{filePathSectionKey}' section.");

            var filePath = filePathSection.Value;
            if (string.IsNullOrWhiteSpace(filePath))
                throw new Exception($"File logger configuration '{filePathSectionKey}' section does not contain a value.");

            var level = LogLevel.None;

            var logLevelSection = configuration.GetSection("LogLevel");
            var defaultLogLevel = logLevelSection?["Default"];

            if (!string.IsNullOrWhiteSpace(defaultLogLevel))
            {
                if (!Enum.TryParse(defaultLogLevel, out level))
                {
                    throw new InvalidOperationException($"Configuration value '{defaultLogLevel}' for category 'Default' is not supported.");
                }
            }

            return AddFile(factory, filePath, level);
        }

        public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath, LogLevel level = LogLevel.Information)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory), $"{nameof(ILoggerFactory)} is null (add Microsoft.Extensions.Logging NuGet package).");

            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            factory.AddProvider(new FileLoggerProvider(filePath, level));

            return factory;
        }
    }
}
