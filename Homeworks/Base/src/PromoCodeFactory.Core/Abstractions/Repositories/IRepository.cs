using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetManyByIdAsync(Guid[] id);
        Task AddAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(Guid id);
    }
}