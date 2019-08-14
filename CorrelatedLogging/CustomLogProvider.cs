using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CorrelatedLogging 
{
    public class CustomLogProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly IHttpContextAccessor _accessor;//DI

        public CustomLogProvider(Func<string, LogLevel, bool> filter, IHttpContextAccessor accessor)
        {
            _filter = filter;
            _accessor = accessor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(categoryName, _filter, _accessor);
        }

        public void Dispose()
        {
        }
    }
}
