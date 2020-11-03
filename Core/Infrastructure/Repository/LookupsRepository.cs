using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppZeroAPI.Models;
using System.Linq.Expressions;

namespace AppZeroAPI.Repository
{

    public class LookupsRepository : BaseRepository, ILookupsRepository
    {
       
        private readonly ILogger<LookupsRepository> logger;
        public LookupsRepository(IConfiguration configuration, ILogger<LookupsRepository> logger) : base(configuration)
        {
            this.logger = logger;
        }


        public async Task<IEnumerable<LookUps>> GetCountries()
        { 
            var sql = "SELECT code, name as value FROM lkp_countries"; 
            logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<LookUps>(sql);
                return result.ToList();
            }
        }
        public async Task<IEnumerable<LookUps>> GetStates()
        {
            var sql = "SELECT code, name as value FROM lkp_states";
            logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<LookUps>(sql);
                return result.ToList();
            }
        }
        public async Task<IEnumerable<LookUps>> GetLanguages()
        {
            var sql = "SELECT code, name as value FROM lkp_languages";
            logger.LogInformation(sql);
            using (var connection = this.GetOpenConnection())
            {
                var result = await connection.QueryAsync<LookUps>(sql);
                return result.ToList();
            }
        }


        public Task<bool> DeleteByIdAsync(string id)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<LookUps>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<LookUps> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }
        //Task<TEntity> GetByIdAsync(string id );
        public Task<int> AddAsync(LookUps entity)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateAsync(LookUps entity)
        {
            throw new NotImplementedException();
        }
    }
}

