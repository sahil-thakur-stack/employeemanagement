using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.Models
{
    public class EmployeeConnection
    {
        public string ConnectionString { get; }

        public EmployeeConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
