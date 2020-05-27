using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IStartupFilterExample
{
    public class SomeMiddleware
    {
        RequestDelegate _next;
        public SomeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            //await httpContext.Response.WriteAsync("Start filter");
            await _next(httpContext);
            await httpContext.Response.WriteAsync("End filter");
        }
    }
}
