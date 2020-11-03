using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using AppZeroAPI.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AppZeroAPI.Repository
{

    public class MailTemplateRepository : BaseRepository, IMailTemplateRepository
    {

        public readonly ILogger<MailTemplateRepository> logger;
        public MailTemplateRepository(IConfiguration configuration, ILogger<MailTemplateRepository> logger) : base(configuration)
        {
            this.logger = logger;
        }
         

        public async Task<MailTemplates> GetTemplate(int templateId)
        {
            using (var connection = this.GetOpenConnection())
            {

                var result = await connection.GetAsync<MailTemplates>(new { id = templateId });
                return result;
            }
        }

        public Task<bool> DeleteByIdAsync(string id)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<MailTemplates>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<MailTemplates> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }
        public Task<int> AddAsync(MailTemplates enity)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateAsync(MailTemplates enity)
        {
            throw new NotImplementedException();
        }
    }
       
}
