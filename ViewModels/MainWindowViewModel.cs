using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Controls;
using ExamSheduleDesign.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ExamSheduleDesign.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;
        private readonly IExamCountNotifier _examCountNotifier;
        private readonly IDataService _dataService;

        [ObservableProperty]
        private int _scheduleCount;

        [ObservableProperty]
        private bool _isServerConnected;

        public MainWindowViewModel(INavigationService navigationService, INotificationService notificationService,
                                   IExamCountNotifier examCountNotifier, IDataService dataService)
        {
            _navigationService = navigationService;
            _notificationService = notificationService;
            _examCountNotifier = examCountNotifier;
            _dataService = dataService;

            _navigationService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(INavigationService.CurrentPage))
                    OnPropertyChanged(nameof(CurrentPage));
                else if (e.PropertyName == nameof(INavigationService.Breadcrumb))
                    OnPropertyChanged(nameof(Breadcrumb));
            };

            _examCountNotifier.CountChanged += (count) => ScheduleCount = count;

            _navigationService.NavigateToMain();

            Task.Run(async () =>
            {
                var count = await _dataService.GetTotalExamsCountAsync();
                Application.Current.Dispatcher.Invoke(() => ScheduleCount = count);
            });

            Task.Run(async () => await RefreshConnectionStatusAsync());
        }

        public UserControl CurrentPage => _navigationService.CurrentPage;
        public string Breadcrumb => _navigationService.Breadcrumb;

        [ObservableProperty]
        private bool _isDevMode = false;

        partial void OnIsDevModeChanged(bool value)
        {
            // Если режим разработчика выключен и текущая страница - DevView, переходим на главную
            if (!value && _navigationService.CurrentPage is Views.DevView)
            {
                _navigationService.NavigateToMain();
            }
        }

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

        [RelayCommand]
        private async Task RefreshConnectionStatusAsync()
        {
            IsServerConnected = await _dataService.CheckServerConnectionAsync();
            if (IsServerConnected)
                _notificationService.Show("Сервер доступен.", NotificationType.Success);
            else
                _notificationService.Show("Сервер недоступен.", NotificationType.Error);
        }
    }
}