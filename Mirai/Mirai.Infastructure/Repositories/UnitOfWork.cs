/*using SportsBicycleStore.Application.Interfaces.Repositories;
using SportsBicycleStore.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsBicycleStore.Infastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IUserRepository UserRepository { get; }


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            UserRepository = new UserRepository(_context);
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
*/