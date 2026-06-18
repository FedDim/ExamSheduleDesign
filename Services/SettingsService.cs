using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        private SettingsData _data;

        public bool IsGroupValidationEnabled
        {
            get => _data.IsGroupValidationEnabled;
            set => _data.IsGroupValidationEnabled = value;
        }

        public SettingsService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appData, "ExamScheduleDesign");
            Directory.CreateDirectory(appFolder);
            _settingsFilePath = Path.Combine(appFolder, "settings.json");
            _data = new SettingsData();
            LoadSync();
        }

        private void LoadSync()
        {
            if (File.Exists(_settingsFilePath))
            {
                string json = File.ReadAllText(_settingsFilePath);
                _data = JsonSerializer.Deserialize<SettingsData>(json) ?? new SettingsData();
            }
            else
            {
                _data = new SettingsData();
                Save();
            }
        }

        public async Task LoadAsync()
        {
            await Task.Run(LoadSync);
        }

        public async Task SaveAsync()
        {
            await Task.Run(Save);
        }

        private void Save()
        {
            string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFilePath, json);
        }

        private class SettingsData
        {
            public bool IsGroupValidationEnabled { get; set; } = true; // по умолчанию включена
        }
    }
}