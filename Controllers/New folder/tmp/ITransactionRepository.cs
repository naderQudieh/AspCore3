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
using System.Linq.Expressions;
using AppZeroAPI.Models;

namespace AppZeroAPI.Repository
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<TransactionDTO>> GetTransactions(Expression<Func<TransactionDTO, bool>> predicate);
        Task<IEnumerable<TransactionDTO>> CreateTransactions(IEnumerable<TransactionDTO> transaction);
    }
}