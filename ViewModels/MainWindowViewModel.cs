using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Services;
using ExamSheduleDesign.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Controls;

namespace ExamSheduleDesign.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private UserControl _currentPage;

        [ObservableProperty]
        private string _breadcrumb = "Главная";

        [ObservableProperty]
        private bool _isDevMode = false;

        [ObservableProperty]
        private string _currentPageTag = "Main";

        private string _addDataTab = "Teacher";
        private string _editDataTab = "Teacher";

        public MainWindowViewModel(IServiceProvider serviceProvider, INotificationService notificationService)
        {
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
            NavigateToMain();
        }

        private void Navigate(string pageTag, UserControl page, string breadcrumb)
        {
            CurrentPage = page;
            CurrentPageTag = pageTag;
            Breadcrumb = breadcrumb;
        }

        [RelayCommand]
        private void NavigateToMain()
        {
            var view = _serviceProvider.GetRequiredService<MainView>();
            Navigate("Main", view, "Главная");
        }

        [RelayCommand]
        private void NavigateToSchedule()
        {
            var view = _serviceProvider.GetRequiredService<ScheduleView>();
            Navigate("Schedule", view, "Таблица расписания");
        }

        [RelayCommand]
        private void NavigateToAdd()
        {
            var view = _serviceProvider.GetRequiredService<AddDataView>();
            if (view.DataContext is AddDataViewModel vm)
                vm.CurrentTab = _addDataTab;
            Navigate("Add", view, "Добавление данных");
        }

        [RelayCommand]
        private void NavigateToAddWithParam(string tab)
        {
            _addDataTab = tab;
            NavigateToAdd();
        }

        [RelayCommand]
        private void NavigateToEdit()
        {
            var view = _serviceProvider.GetRequiredService<EditDataView>();
            if (view.DataContext is EditDataViewModel vm)
                vm.CurrentTab = _editDataTab;
            Navigate("Edit", view, "Редактирование данных");
        }

        [RelayCommand]
        private void NavigateToEditWithParam(string tab)
        {
            _editDataTab = tab;
            NavigateToEdit();
        }

        [RelayCommand]
        private void NavigateToDev()
        {
            var view = _serviceProvider.GetRequiredService<DevView>();
            Navigate("Dev", view, "Окно разработчика");
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            var view = _serviceProvider.GetRequiredService<SettingsView>();
            Navigate("Settings", view, "Настройки");
        }
    }
}