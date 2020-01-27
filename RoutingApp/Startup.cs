using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RoutingApp
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RouteBuilder routeBuilder = new RouteBuilder(app);
            routeBuilder.MapRoute("{controller}/{action}/{id?}", HandleRouteData);
            routeBuilder.MapRoute("/", Handle);
            app.UseRouter(routeBuilder.Build());
        }

        async Task Handle(HttpContext ctx)
        {
            await ctx.Response.WriteAsync("Hello routing)");
        }

        async Task HandleRouteData(HttpContext ctx)
        {
            RouteData routeData = ctx.GetRouteData();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Routers:");
            foreach(var r in routeData.Routers)
            {
                stringBuilder.AppendLine("route: " + r.ToString());
            }

            stringBuilder.AppendLine("Сегменты:");
            foreach(var rd in routeData.Values)
            {
                stringBuilder.AppendFormat("{0} - {1}", rd.Key, rd.Value);
                stringBuilder.AppendLine();
            }
            ctx.Response.ContentType = "text/plain; charset=utf-8";
            await ctx.Response.WriteAsync(stringBuilder.ToString());
        }
    }
}
