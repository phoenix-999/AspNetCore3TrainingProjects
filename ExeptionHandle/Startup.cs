using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExeptionHandle
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            env.EnvironmentName = "Production";
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            //app.UseStatusCodePages("text/html", "Error. StatusCore {0}");//��������� ��������� ��� ������ ������� � ����� ��������� ������

            //app.UseStatusCodePagesWithRedirects("/error?code={0}"); //������ ��� ��������� � ��������������� ������� ������ ������� ��������� ��� 302 / Found.
            //�� ���� ��������� �������������� ������ ����� ������������, ������ ��������� ��� 302 ����� ���������, ��� ������ ��������� �� ������ ����� - �� ���� "/error/?code=404".
            //�������� ��������� ����� ���� ��������, �������� � ����� ������ ��������� ����������

            app.UseStatusCodePagesWithReExecute("/error", "?code={0}");//��������� ���������� � ���������������, �� ������ ������� ��� 404


            app.Map("/error", appBuilder =>
            {
                appBuilder.Run(async ctx =>
                {
                    ctx.Response.ContentType = "text/html; charset=utf-8";

                    int statusCode = ctx.Response.StatusCode;
                    if (statusCode >= 500)
                    {
                        await ctx.Response.WriteAsync("������ ���������� ����");
                        return;
                    }
                    else if (ctx.Request.Query["code"].ToString() != null)
                    {
                        await ctx.Response.WriteAsync($"Error. Code = {ctx.Request.Query["code"]}");
                        return;
                    }
                });
            });

            app.Map("/500", appBuilder => 
            {
                /*���� ������������� ������ � �������� Action<IApplicationBuilder>, ������� ����������� �� ������ �������� app.Map ��� ������ ���������� ��� ������� ����������.
                 * �������, ��� �������� � ������ Map ������������ ������ ��� ��� ������� ����������, � �� ��� ��������� �� ���������� ����
                 * �������� ��� ������� � ������������ �������� ������ Map
                 */
                appBuilder.Run(async ctx =>
                {
                    ctx.Response.StatusCode = 500;
                    await Task.FromResult(0);
                });

            });

        }
    }
}
