using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloApp.Infrastructure
{
    public static class TokenExtension
    {
        public static IApplicationBuilder UseToken(this IApplicationBuilder app, string pattern)
        {
            return app.UseMiddleware<TokenMiddleware>(pattern);
        }
    }
}
