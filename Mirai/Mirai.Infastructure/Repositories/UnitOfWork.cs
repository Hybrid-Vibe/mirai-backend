using Mirai.Application.Interfaces.Repositories;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Repositories
{
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

        public IOrderRepository OrderRepository {  get; }

        public IPaymentRepository PaymentRepository { get; }

        public ICartItemsRepository CartItemsRepository { get; }

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
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
       }
    }
}
