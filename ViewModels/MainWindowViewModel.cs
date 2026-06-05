using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Services;
using System.Windows.Controls;

namespace ExamSheduleDesign.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;

        public MainWindowViewModel(INavigationService navigationService, INotificationService notificationService)
        {
            _navigationService = navigationService;
            _notificationService = notificationService;

            // Подписываемся на изменения навигационного сервиса, чтобы обновлять привязки в XAML
            _navigationService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(INavigationService.CurrentPage))
                    OnPropertyChanged(nameof(CurrentPage));
                else if (e.PropertyName == nameof(INavigationService.Breadcrumb))
                    OnPropertyChanged(nameof(Breadcrumb));
            };

            _navigationService.NavigateToMain();
        }

        public UserControl CurrentPage => _navigationService.CurrentPage;
        public string Breadcrumb => _navigationService.Breadcrumb;

        [ObservableProperty]
        private bool _isDevMode = false;

        [RelayCommand]
        private void NavigateToMain() => _navigationService.NavigateToMain();

        [RelayCommand]
        private void NavigateToSchedule() => _navigationService.NavigateToSchedule();

        [RelayCommand]
        private void NavigateToAdd() => _navigationService.NavigateToAdd();

        [RelayCommand]
        private void NavigateToAddWithParam(string tab) => _navigationService.NavigateToAdd(tab);

        [RelayCommand]
        private void NavigateToEdit() => _navigationService.NavigateToEdit();

        [RelayCommand]
        private void NavigateToEditWithParam(string tab) => _navigationService.NavigateToEdit(tab);

        [RelayCommand]
        private void NavigateToDev() => _navigationService.NavigateToDev();

        [RelayCommand]
        private void NavigateToSettings() => _navigationService.NavigateToSettings();
    }
}