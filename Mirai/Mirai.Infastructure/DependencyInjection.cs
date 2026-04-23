using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Infastructure.Data;
using Mirai.Infastructure.Repositories;
using Mirai.Infastructure.Services;
using SportsBicycleStore.Infastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }

        public static IServiceCollection AddApplication(
           this IServiceCollection services)
        {
            // Services
            //services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
