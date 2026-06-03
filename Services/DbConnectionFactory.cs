using ExamSheduleDesign.Services.Interfaces;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace ExamSheduleDesign.Services
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _serverConnectionString;
        private readonly string _localConnectionString;

        public DbConnectionFactory()
        {
            _serverConnectionString = ConfigurationManager.ConnectionStrings["ExamScheduleServer"].ConnectionString;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataFolder = Path.Combine(baseDir, "Data");
            Directory.CreateDirectory(dataFolder);
            string localPath = Path.Combine(dataFolder, "LocalExams.db");
            _localConnectionString = $"Data Source={localPath};Version=3;Journal Mode=Delete;Pooling=False;BusyTimeout=30000;";
        }

        public SqlConnection CreateServerConnection() => new SqlConnection(_serverConnectionString);
        public SQLiteConnection CreateLocalConnection() => new SQLiteConnection(_localConnectionString);
    }
}
