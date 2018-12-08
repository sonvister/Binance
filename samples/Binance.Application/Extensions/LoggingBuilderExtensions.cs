using System;
using Binance.Application.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.Application
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, IConfiguration configuration)
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

            // ReSharper disable once InvertIf
            if (!string.IsNullOrWhiteSpace(defaultLogLevel))
            {
                if (!Enum.TryParse(defaultLogLevel, out level))
                {
                    throw new InvalidOperationException($"Configuration value '{defaultLogLevel}' for category 'Default' is not supported.");
                }
            }

            return AddFile(builder, filePath, level);
        }

        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath, LogLevel level = LogLevel.Information)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder), $"{nameof(ILoggerFactory)} is null (add Microsoft.Extensions.Logging NuGet package).");

            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            builder.AddProvider(new FileLoggerProvider(filePath, level));

            return builder;
        }
    }
}
