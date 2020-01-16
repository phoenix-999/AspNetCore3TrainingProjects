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


            app.UseDirectoryBrowser(new DirectoryBrowserOptions //����������� ��������� ��������� �� ������������� �������� � ������ ���������� �������� � ��������
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "static")),//��������� ���� � �������� ���������
                RequestPath = "/dir" //��������� �������� ���� ������� ��� ��������� ��������
            }) ;

            //app.UseDefaultFiles();//��������� ������������� ���������� ������� �� ��������� ��� ��������� � ��� ��������
            var defaultFileOptions = new DefaultFilesOptions();
            defaultFileOptions.DefaultFileNames.Insert(0, "index.html");
            app.UseDefaultFiles(defaultFileOptions);//���� �����, ��� � ���������� �� � ������� ���������� �������� ������ �� ����������

            app.UseStaticFiles(); // ������������ ��� ������� � wwwroot
            app.UseStaticFiles(new StaticFileOptions() // ��������������� �������� �������� ������ � ������������ �����������
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\")),
                RequestPath = new PathString("/pages") // �� ������� /pages/index.html �� ����� ���������� � ����� wwwroot/index.html.
            });

            app.UseFileServer(enableDirectoryBrowsing: true); //���������� ���������������� ����� ���� ���� ������������� ������� UseStaticFiles, UseDefaultFiles � UseDirectoryBrowser. ���� ���������� ��� ��������� ���� ����������.

            app.UseToken("not access");

            int x = 5;
            int y = 8;
            int z = 0;

            app.Map("/mapindex", appBuilder => appBuilder.Run(async ctx => await ctx.Response.WriteAsync("Map Index") ) );//���������� ������� �� ��������� ������� ��������
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
                await next();//����� ���������� ���������� middleware (� �.�.  ����� Run), ������� ����� ������������ � ������� Func<Task>
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync($"{x} * {y} = {z}");
            });
        }
    }
}
