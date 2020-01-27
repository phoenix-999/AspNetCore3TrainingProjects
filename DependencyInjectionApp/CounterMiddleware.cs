using DependencyInjectionApp.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionApp
{
    public class CounterMiddleware
    {
        RequestDelegate next;
        int i; //счетчик запросов
        public CounterMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext ctx, ICounter counter, CounterService counterService)
        {
            i++;
            ctx.Response.ContentType = "text/html;charset=utf-8";
            await ctx.Response.WriteAsync($"Запрос {i}; Counter: {counter.Value}; Service: {counterService.Counter.Value}");
        }
    }
}
