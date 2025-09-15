using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }
        public Task<IEnumerable<T>> GetManyByIdAsync(Guid[] ids)
        {
            return Task.FromResult(Data.Where(x => ids.Contains(x.Id)));
        }
        public async Task AddAsync(T item)
        {
            await Task.Run(() =>
            {
                Data = Data.Append(item);
            });
        }

        public async Task UpdateAsync(T item)
        {
            await Task.Run(() =>
            {
                Data = Data.Select(x => x.Id == item.Id ? item : x);
            });            
        }

        public async Task DeleteAsync(Guid id)
        {
            await Task.Run(() =>
            {
                Data = Data.Where(x => x.Id != id);
            });
        }
    }
}