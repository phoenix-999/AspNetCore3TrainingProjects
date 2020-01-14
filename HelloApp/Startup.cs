using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HelloApp
{
    public class Startup
    {
        IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            int x = 5;
            int y = 8;
            int z = 0;

            app.Map("/index", appBuilder => appBuilder.Run(async ctx => await ctx.Response.WriteAsync("Index") ) );//отображает маршрут на отдельный конвеер запросов
            app.Map("/home", home =>
            {
                home.Map("/about", appBuilder => appBuilder.Run(async ctx => await ctx.Response.WriteAsync("Home/About") ) );
            });

            app.Use(async (context, next) =>
            {
                z = x * y;
                await next();
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync($"{x} * {y} = {z}");
            });
        }
    }
}
