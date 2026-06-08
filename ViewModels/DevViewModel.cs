using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Controls;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ExamSheduleDesign.ViewModels
{
    public partial class DevViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly IConnectionSettingsService _connectionSettings;

        [ObservableProperty]
        private int _generationCount = 10;

        [ObservableProperty]
        private string _statusText = "Готов к генерации";

        [ObservableProperty]
        private bool _isGenerating;

        [ObservableProperty]
        private ObservableCollection<Exam> _generatedExams = new();

        [ObservableProperty]
        private int _teachersCount;

        [ObservableProperty]
        private int _disciplinesCount;

        [ObservableProperty]
        private int _groupsCount;

        [ObservableProperty]
        private string _server;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _databaseName;

        public DevViewModel(IDataService dataService, INotificationService notificationService,
                            IConnectionSettingsService connectionSettings)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            _connectionSettings = connectionSettings;
            _ = LoadConnectionSettingsAsync();
        }

        private async Task LoadConnectionSettingsAsync()
        {
            await _connectionSettings.LoadAsync();
            Server = _connectionSettings.Server;
            Username = _connectionSettings.Username;
            Password = _connectionSettings.Password;
            DatabaseName = _connectionSettings.DatabaseName;
        }

        public async Task LoadDataAsync()
        {
            await RefreshGeneratedListAsync();
            await LoadCountsAsync();
        }

        private async Task RefreshGeneratedListAsync()
        {
            var allExams = await _dataService.GetExamsAsync();
            var lastExams = allExams.Skip(Math.Max(0, allExams.Count - 50)).ToList();
            GeneratedExams = new ObservableCollection<Exam>(lastExams);
        }

        private async Task LoadCountsAsync()
        {
            TeachersCount = (await _dataService.GetTeachersAsync()).Count;
            DisciplinesCount = (await _dataService.GetDisciplinesAsync()).Count;
            GroupsCount = (await _dataService.GetGroupsAsync()).Count;
        }

        [RelayCommand]
        private async Task TestLoadExamsAsync()
        {
            try
            {
                var exams = await _dataService.GetExamsAsync();
                _notificationService.Show($"Загружено экзаменов: {exams.Count}", NotificationType.Information);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private async Task GenerateAsync()
        {
            if (IsGenerating) return;
            IsGenerating = true;
            StatusText = "Генерация...";
            await _dataService.GenerateExamsAsync(GenerationCount);
            await RefreshGeneratedListAsync();
            await LoadCountsAsync();
            StatusText = $"Сгенерировано {GenerationCount} экзаменов";
            IsGenerating = false;
        }

        [RelayCommand]
        private async Task ClearGeneratedAsync()
        {
            if (MessageBox.Show("Удалить все сгенерированные экзамены?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await _dataService.ClearGeneratedExamsAsync();
                await RefreshGeneratedListAsync();
                await LoadCountsAsync();
                StatusText = "Сгенерированные экзамены удалены";
            }
        }

        [RelayCommand]
        private async Task SaveConnectionSettingsAsync()
        {
            _connectionSettings.Server = Server;
            _connectionSettings.Username = Username;
            _connectionSettings.Password = Password;
            _connectionSettings.DatabaseName = DatabaseName;
            await _connectionSettings.SaveAsync();
            await _dataService.ReconnectAsync();
        }
    }
}