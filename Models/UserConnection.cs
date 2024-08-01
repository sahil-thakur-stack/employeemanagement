using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.Models
{
    public class UserConnection
    {
        public string ConnectionString { get; }

        public UserConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
