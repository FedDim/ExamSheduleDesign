using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Services;
using System.Threading.Tasks;

namespace ExamSheduleDesign.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;

        public SettingsViewModel(IDataService dataService, INotificationService notificationService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
        }

        [RelayCommand]
        private async Task ReconnectAsync()
        {
            await _dataService.ReconnectAsync();
        }
    }
}