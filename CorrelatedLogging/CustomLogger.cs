using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CorrelatedLogging
{
    public class CustomLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        private readonly IHttpContextAccessor _accessor;

        public CustomLogger(string categoryName, Func<string, LogLevel, bool> filter, IHttpContextAccessor accessor)
        {
            _categoryName = categoryName;
            _filter = filter;
            _accessor = accessor;
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
            if (_accessor.HttpContext != null) // you should check HttpContext 
            {
                var headers = _accessor.HttpContext.Request.Headers["CorrelationId"].ToString();
                if (headers != "")
                {
                    message += Environment.NewLine + headers + Environment.NewLine + _accessor.HttpContext.Request.Path;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(message);
                }

            }
        }
    }

    public static class CustomLoggerExtensions
    {
        public static ILoggerFactory AddCustomLogger(this ILoggerFactory factory, IHttpContextAccessor accessor,
                                              Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new CustomLogProvider(filter, accessor));
            return factory;
        }
    }
}
