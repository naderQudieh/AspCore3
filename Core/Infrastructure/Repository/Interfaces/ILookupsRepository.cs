using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface ILookupsRepository : IGenericRepository<LookUps>
    {
        Task<IEnumerable<LookUps>> GetCountries();
        Task<IEnumerable<LookUps>> GetStates();
        Task<IEnumerable<LookUps>> GetLanguages();

    }
}
