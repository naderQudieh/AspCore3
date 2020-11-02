using AppZeroAPI.Entities ;
using AppZeroAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IMailTemplateRepository : IGenericRepository<MailTemplates>
    {
        Task<MailTemplates> GetTemplate(int templateType);
    }
    
}