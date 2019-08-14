using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWiz.LoggingMiddleware
{
    public class CorralatedLogProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;

        public CorralatedLogProvider(Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CorralatedLogger(categoryName, _filter, Correlation.Id.Value);
        }

        public void Dispose()
        {
        }
    }
}
