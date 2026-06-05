using System;
using System.IO;

namespace ExamSheduleDesign.Services
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConnectionSettingsService _connectionSettings;
        private string _serverConnectionString;
        private string _localConnectionString;

        public ConnectionStringProvider(IConnectionSettingsService connectionSettings)
        {
            _connectionSettings = connectionSettings;
            Refresh();
        }

        public string GetServerConnectionString() => _serverConnectionString;
        public string GetLocalConnectionString() => _localConnectionString;

        public void Refresh()
        {
            _serverConnectionString = $"Server={_connectionSettings.Server};" +
                                      $"Database={_connectionSettings.DatabaseName};" +
                                      $"User Id={_connectionSettings.Username};" +
                                      $"Password={_connectionSettings.Password};" +
                                      $"TrustServerCertificate=True;Encrypt=False;Application Name=ExamScheduleApp;" +
                                      $"Pooling=True;Max Pool Size=100;Connect Timeout=10;";

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataFolder = Path.Combine(baseDir, "Data");
            Directory.CreateDirectory(dataFolder);
            string localPath = Path.Combine(dataFolder, "LocalExams.db");
            _localConnectionString = $"Data Source={localPath};Version=3;Journal Mode=Delete;Pooling=False;BusyTimeout=30000;";
        }
    }
}