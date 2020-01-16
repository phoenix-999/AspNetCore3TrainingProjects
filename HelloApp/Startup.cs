using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HelloApp.Infrastructure;
using Microsoft.Extensions.FileProviders;
using System.IO;

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


            app.UseDirectoryBrowser(new DirectoryBrowserOptions //Возможность просмотра каталогов по определенному маршруту с ручной установкой каталога и маршрута
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "static")),//Установка пути к каталогу просмотра
                RequestPath = "/dir" //Установка сегмента пути запроса для просмотра каталога
            }) ;

            //app.UseDefaultFiles();//Включение распознавания стаических страниц по умолчанию без обращения к ним напрямую
            var defaultFileOptions = new DefaultFilesOptions();
            defaultFileOptions.DefaultFileNames.Insert(0, "index.html");
            app.UseDefaultFiles(defaultFileOptions);//Тоже самое, что и предыдущее но с уручной установкой названия файлов по уммолчанию

            app.UseStaticFiles(); // обрабатывает все запросы к wwwroot
            app.UseStaticFiles(new StaticFileOptions() // Перенаправление значения адресной строки к статическому содержимому
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\")),
                RequestPath = new PathString("/pages") // по запросу /pages/index.html мы можем обратиться к файлу wwwroot/index.html.
            });

            app.UseFileServer(enableDirectoryBrowsing: true); //объединяет функциональность сразу всех трех вышеописанных методов UseStaticFiles, UseDefaultFiles и UseDirectoryBrowser. Есть перегрузка для настройки всех параметров.

            app.UseToken("not access");

            int x = 5;
            int y = 8;
            int z = 0;

            app.Map("/mapindex", appBuilder => appBuilder.Run(async ctx => await ctx.Response.WriteAsync("Map Index") ) );//отображает маршрут на отдельный конвеер запросов
            app.Map("/maphome", home =>
            {
                home.Map("/mapabout", appBuilder => appBuilder.Run(async ctx => await ctx.Response.WriteAsync("Map Home/Map About") ) );
            });

            app.MapWhen(ctx => {
                return ctx.Request.Query.ContainsKey("id") && ctx.Request.Query["id"] == "1";
            }, appBuilder => {
                appBuilder.Run(async ctx => await ctx.Response.WriteAsync("id = 1"));
            });

            app.Use(async (context, next) =>
            {
                z = x * y;
                await next();//Вызов следующого компонента middleware (в т.ч.  метод Run), который будет преобразован в делегат Func<Task>
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync($"{x} * {y} = {z}");
            });
        }
    }
}
