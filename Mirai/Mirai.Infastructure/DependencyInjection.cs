using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Infastructure.Data;
using Mirai.Infastructure.Repositories;
using Mirai.Infastructure.Services;

namespace Mirai.Infastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddPersistence(configuration);
            services.AddApplicationServices();
            services.AddExternalClients(configuration);

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()
                )
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICartItemsService, CartItemsService>();
            services.AddScoped<IAIImageService, AIImageService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ITurnstileService, TurnstileService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IFlashSaleService, FlashSaleService>();

            return services;
        }

        private static IServiceCollection AddExternalClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<INanoBananaService, NanoBananaService>(client =>
            {
                var baseUrl = configuration["NanoBanana:BaseUrl"] ?? "https://api.nanobanana.ai";
                client.BaseAddress = new Uri(baseUrl);
            });

            return services;
        }
    }
}
