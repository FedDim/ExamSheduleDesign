using ExamSheduleDesign.Services.Interfaces;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace ExamSheduleDesign.Services
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public DbConnectionFactory(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public SqlConnection CreateServerConnection()
            => new SqlConnection(_connectionStringProvider.GetServerConnectionString());

        public SQLiteConnection CreateLocalConnection()
            => new SQLiteConnection(_connectionStringProvider.GetLocalConnectionString());
    }
}