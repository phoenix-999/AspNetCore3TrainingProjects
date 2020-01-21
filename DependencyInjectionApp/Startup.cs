using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionApp.Services;
using DependencyInjectionApp.Services.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMessageSender>(provider => {

                if (DateTime.Now.Hour >= 12) return new EmailMessageSender();
                else return new SmsMessageSender();
            });
            services.AddTimeService();

            services.AddTransient<ICounter, RandomCounter>();
            services.AddTransient<CounterService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IMessageSender sender, TimeService timeService)
        {
            app.UseMiddleware<CounterMiddleware>();

            app.Run(async ctx =>
            {
                ctx.Response.ContentType = "text/plain;charset=utf-8";
                await ctx.Response.WriteAsync($"Сообщение: {sender.Send()}, {timeService.GetTime()}");
            });
            
        }
    }
}
