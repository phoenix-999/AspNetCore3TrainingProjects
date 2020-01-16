using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloApp.Infrastructure
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string pattern;
        public TokenMiddleware(RequestDelegate next, string pattern)
        {
            this.next = next;
            this.pattern = pattern;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            var token = ctx.Request.Query["token"];
            if(token == pattern)
            {
                ctx.Response.StatusCode = 403;
                await ctx.Response.WriteAsync("Access Denied. Token is invalid");
            }
            else
            {
                await next(ctx);
            }
        }
    }
}
