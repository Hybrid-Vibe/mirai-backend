using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mirai.Application.DTO;
using Mirai.Application.DTO.Admin;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Infastructure.Data;
using Mirai.Infastructure.Repositories;
using Mirai.Infastructure.Services;
using System.Net.Http.Headers;

namespace Mirai.Infastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddPersistence(configuration); 
            services.AddApplicationServices(configuration);

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

        private static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
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
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ITurnstileService, TurnstileService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IFlashSaleService, FlashSaleService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IPromptOptimizerService, PromptOptimizerService>();
            services.Configure<GroqOptions>(configuration.GetSection("Groq"));
            services.AddScoped<ILanguageDetector, LanguageDetector>();
            services.AddScoped<Mirai.Application.Interfaces.ICollectionService, CollectionService>();
            services.AddSingleton<SupabaseClientService>();
            services.AddHttpClient("ExternalMedia");
            services.Configure<ReplicateOptions>(
                configuration.GetSection("Replicate")
            );
            services.AddHttpClient<IReplicateImageService, ReplicateImageService>(
                (sp, client) =>
                {
                    var options =
                        sp.GetRequiredService<IOptions<ReplicateOptions>>()
                        .Value;


                    client.BaseAddress =
                        new Uri(options.BaseUrl);


                    client.Timeout =
                        TimeSpan.FromMinutes(3);


                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            options.ApiToken
                        );
                });

            return services;
        }

        
    }
}
