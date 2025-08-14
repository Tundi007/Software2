using Microsoft.Extensions.DependencyInjection;
using StorageService.Application.Interfaces;
using StorageService.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IStorageTypeService, StorageTypeService>();
            services.AddTransient<IUserStorageService, UserStorageService>();
            return services;
        }
    }
}
