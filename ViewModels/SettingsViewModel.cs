using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Controls;
using ExamSheduleDesign.Services;
using System;
using System.Threading.Tasks;

namespace ExamSheduleDesign.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private bool _isGroupValidationEnabled;

        public SettingsViewModel(IDataService dataService, INotificationService notificationService, ISettingsService settingsService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            _settingsService = settingsService;
            _ = LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            await _settingsService.LoadAsync();
            IsGroupValidationEnabled = _settingsService.IsGroupValidationEnabled;
        }

        partial void OnIsGroupValidationEnabledChanged(bool value)
        {
            _settingsService.IsGroupValidationEnabled = value;
            _ = _settingsService.SaveAsync();
        }

        [RelayCommand]
        private async Task ReconnectAsync()
        {
            try
            {
                await _dataService.ReconnectAsync();
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка переподключения: {ex.Message}", NotificationType.Error);
            }
        }
    }
}