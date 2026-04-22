using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        List<T> GetAll();
        Task<List<T>> GetAllAsync();

        T GetById(int id);
        T GetById(string code);
        T GetById(Guid id);

        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string code);
        Task<T> GetByIdAsync(Guid id);

        void Create(T entity);
        Task<int> CreateAsync(T entity);

        void Update(T entity);
        Task<int> UpdateAsync(T entity);

        bool Remove(T entity);
        Task<bool> RemoveAsync(T entity);

        void PrepareCreate(T entity);
        void PrepareUpdate(T entity);
        void PrepareRemove(T entity);

        int Save();
        Task<int> SaveAsync();
    }
}
