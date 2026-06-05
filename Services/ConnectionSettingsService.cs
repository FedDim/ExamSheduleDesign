using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public class ConnectionSettingsService : IConnectionSettingsService
    {
        private readonly string _filePath;
        private ConnectionData _data;

        public string Server { get => _data.Server; set => _data.Server = value; }
        public string Username { get => _data.Username; set => _data.Username = value; }
        public string Password { get => _data.Password; set => _data.Password = value; }
        public string DatabaseName { get => _data.DatabaseName; set => _data.DatabaseName = value; }

        public ConnectionSettingsService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appData, "ExamScheduleDesign");
            Directory.CreateDirectory(appFolder);
            _filePath = Path.Combine(appFolder, "connection.json");
            _data = new ConnectionData();
        }

        public async Task LoadAsync()
        {
            await Task.Run(() =>
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    _data = JsonSerializer.Deserialize<ConnectionData>(json) ?? new ConnectionData();
                }
                else
                {
                    ResetToDefaults();
                    Save();
                }
            });
        }

        public async Task SaveAsync()
        {
            await Task.Run(Save);
        }

        private void Save()
        {
            string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void ResetToDefaults()
        {
            _data = new ConnectionData
            {
                Server = "26.102.173.248",
                Username = "sa",
                Password = "",
                DatabaseName = "ExamScheduleDB"
            };
        }

        private class ConnectionData
        {
            public string Server { get; set; } = "";
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public string DatabaseName { get; set; } = "";
        }
    }
}