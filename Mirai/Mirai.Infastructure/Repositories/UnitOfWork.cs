using Mirai.Application.Interfaces.Repositories;
using Mirai.Infastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Infastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository UserRepository { get; }

    public IAddressRepository AddressRepository { get; }

    public IBrandRepository BrandRepository { get; }

    public ICategoryRepository CategoryRepository { get; }

    public IProductRepository ProductRepository { get; }

    public IProductVariantRepository ProductVariantRepository { get; }

    public IProductImageRepository ProductImageRepository { get; }

    public IOrderRepository OrderRepository { get; }

    public IPaymentRepository PaymentRepository { get; }

    public ICartItemsRepository CartItemsRepository { get; }

    public IAIImageRepository AIImageRepository { get; }

    public IReviewRepository ReviewRepository { get; }
    public IFlashSaleRepository FlashSaleRepository { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        UserRepository = new UserRepository(_context);
        AddressRepository = new AddressRepository(_context);
        BrandRepository = new BrandRepository(_context);
        CategoryRepository = new CategoryRepository(_context);
        ProductRepository = new ProductRepository(_context);
        ProductVariantRepository = new ProductVariantRepository(_context);
        ProductImageRepository = new ProductImageRepository(_context);
        OrderRepository = new OrderRepository(_context);
        PaymentRepository = new PaymentRepository(_context);
        CartItemsRepository = new CartItemsRepository(_context);
        AIImageRepository = new AIImageRepository(_context);
        ReviewRepository = new ReviewRepository(_context);
        FlashSaleRepository = new FlashSaleRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
