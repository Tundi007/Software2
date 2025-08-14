using Microsoft.Extensions.DependencyInjection;
using StorageService.Application.Interfaces;
using StorageService.Infrastructure.Context;
using StorageService.Infrastructure.Repositories;
using StorageService.Infrastructure.Services;

namespace StorageService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<ICloudStorageService, MinioStorageService>();
            services.AddTransient<IStorageTypeRepository, StorageTypeRepository>();
            services.AddTransient<IUserStorageRepository, UserStorageRepository>();
            return services;
        }
    }
}