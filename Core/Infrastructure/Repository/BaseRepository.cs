using AppZeroAPI.Interfaces;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace AppZeroAPI.Repository
{
    public   class BaseRepository
    {
        
        private readonly IConfiguration configuration;
         
        public BaseRepository(IConfiguration configuration )
        {
            
            this.configuration = configuration;

        }
       
        public string DbConnection
        {
            get
            {
                return this.configuration.GetConnectionString("DefaultConnection");
            }
        }
        public IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(DbConnection);
            connection.Open();
            return connection;
        }
   

    }

   
}
