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
    }
}
