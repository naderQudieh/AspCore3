using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    { 
        Task<bool> DeleteByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int id); 
        Task<int> AddAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);

    }
}
