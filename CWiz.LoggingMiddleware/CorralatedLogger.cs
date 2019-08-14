using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWiz.LoggingMiddleware
{
    public class CorralatedLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        private readonly string corralation;

        public CorralatedLogger(string categoryName, Func<string, LogLevel, bool> filter, string corralation)
        {
            _categoryName = categoryName;
            _filter = filter;
            this.corralation = corralation;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{ logLevel }: {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            if (string.IsNullOrEmpty(this.corralation))
            {
                message += Environment.NewLine + "No Correlation";
            }
            else
            {
                message += Environment.NewLine + "Correlation " + this.corralation;
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(message);
        }
    }

    public static class CorralatedLoggerExtensions
    {
        public static ILoggerFactory AddCorralatedLogger(this ILoggerFactory factory,
                                              Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new CorralatedLogProvider(filter));
            return factory;
        }
    }
}
