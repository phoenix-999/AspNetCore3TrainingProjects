using LogsAndStrategy.Models;
using LogsAndStrategy.StorageRepositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Infrastructure
{
    public static class ServicesExtensions
    {
        public static IServiceCollection UseStorageRepositories(this IServiceCollection services)
        {
            services.AddScoped<IItemRepository, ItemRepositoty>();

            return services;
        }
    }
}
