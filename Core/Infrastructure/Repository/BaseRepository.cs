using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AppZeroAPI.Repository
{
    public class BaseRepository
    {

        private readonly IConfiguration configuration;

        public BaseRepository(IConfiguration configuration)
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
