using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HttpsApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Методы Add... используются для настройки опций middleware. Они не обязательны.
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect; //По уммолчанию
                options.HttpsPort = 44344;//Visual Studio игнорирует. Сама устанавливает порт
            });

            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                options.Preload = true;
                options.ExcludedHosts.Add("us.example.com"); //добавляет список доменов, которые надо исключить из действия заголовка.
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();//вызывается, если только приложение уже развернуто для полноценного использования, потому что в процессе разработки использование данного метода может создавать неудобства, так как заголовки кэшируются
            }

            app.UseHttpsRedirection();

            app.Run(async ctx =>
            {
                await ctx.Response.WriteAsync("Hello Wlorld!");
            });
        }
    }
}
