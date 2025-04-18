using System.Data;
using System.Data.SqlClient; // 👈 For SQL Server
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace AMS.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        //public IDbConnection CreateConnection() => new SqlConnection(_connectionString); // 👈 SQL Server connection

        public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
