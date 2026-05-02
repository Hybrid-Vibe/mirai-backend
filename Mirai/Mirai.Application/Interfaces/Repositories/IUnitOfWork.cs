using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
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
    }
}
