using AppZeroAPI.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
  
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(string rec_id);
        Task<bool> DeleteByIdAsync(string rec_id);
        Task<IEnumerable<TEntity>> GetAllAsync(); 
     
        Task<int> AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
       
    }

    
}
