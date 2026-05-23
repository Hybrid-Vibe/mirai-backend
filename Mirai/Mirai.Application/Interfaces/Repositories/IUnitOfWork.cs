using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IUserRepository UserRepository { get; }
        IAddressRepository AddressRepository { get; }
        IBrandRepository BrandRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductVariantRepository ProductVariantRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        ICartItemsRepository CartItemsRepository { get; }
        IAIImageRepository AIImageRepository { get; }
        IReviewRepository ReviewRepository { get; }
    }
}
