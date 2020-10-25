using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<bool> DeleteByIdAsync(long id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(long id);
        //Task<TEntity> GetByIdAsync(string id );
        Task<int> AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
       
    }

    
}
