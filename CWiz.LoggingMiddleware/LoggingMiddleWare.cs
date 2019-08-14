using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CWiz.LoggingMiddleware
{
    public class LoggingMiddleWare
    {
        RequestDelegate _next;
        public LoggingMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            Correlation.Id.Value = context.Request.Headers["CorrelationId"];
            return _next.Invoke(context);
        }
    };

    public static class LoggingMiddleWareExtensions
    {
        public static IApplicationBuilder UseCorralationLogger(this IApplicationBuilder builder)
            => builder.UseMiddleware<LoggingMiddleWare>();
    }
}
