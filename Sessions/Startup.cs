using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sessions
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.Configure<SessionOptions>(opt => //Настройка сессий
            {
                opt.Cookie.Name = "My session";
                opt.Cookie.IsEssential = true;
                opt.IdleTimeout = TimeSpan.FromSeconds(5);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            app.Run(async ctx =>
            {
                if (ctx.Session.Keys.Contains("person"))
                {
                    Person person = ctx.Session.Get<Person>("person");
                    await ctx.Response.WriteAsync($"Hello {person?.Name}, your age: {person?.Age}!");
                }
                else
                {
                    Person person = new Person { Name = "Tom", Age = 28};
                    ctx.Session.Set<Person>("person", person);
                    await ctx.Response.WriteAsync("Hello World");
                }
            });

            //app.Run(async ctx =>
            //{
            //    ctx.Response.ContentType = "text/plain;charset=utf-8";
            //    if(ctx.Session.Keys.Contains("name"))
            //    {
            //        await ctx.Response.WriteAsync($"Hello {ctx.Session.GetString("name")}");
            //    }
            //    else
            //    {
            //        ctx.Session.SetString("name", "Bob");
            //        await ctx.Response.WriteAsync("Hello world!");
            //    }
            //});
        }
    }
}
