using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CWiz.LoggingMiddleware
{
    static class Correlation
    {
        public static AsyncLocal<string> Id { get; } = new AsyncLocal<string>();
    }
}
