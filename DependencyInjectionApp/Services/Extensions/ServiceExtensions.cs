using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionApp.Services.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddTimeService(this IServiceCollection services)
        {
            services.AddSingleton<TimeService>();
        }
    }
}
