using System.Data.SqlClient;
using System.Data.SQLite;

namespace ExamSheduleDesign.Services.Interfaces
{
    public interface IDbConnectionFactory
    {
        SqlConnection CreateServerConnection();
        SQLiteConnection CreateLocalConnection();
    }
}
