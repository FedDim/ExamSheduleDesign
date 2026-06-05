using ExamSheduleDesign.ViewModels;
using ExamSheduleDesign.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace ExamSheduleDesign.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private UserControl _currentPage;
        private string _breadcrumb;

        public UserControl CurrentPage
        {
            get => _currentPage;
            private set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public string Breadcrumb
        {
            get => _breadcrumb;
            private set
            {
                _breadcrumb = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Navigate(UserControl page, string breadcrumb)
        {
            CurrentPage = page;
            Breadcrumb = breadcrumb;
        }

        public void NavigateToMain()
        {
            var view = _serviceProvider.GetRequiredService<MainView>();
            Navigate(view, "Главная");
        }

        public void NavigateToSchedule()
        {
            var view = _serviceProvider.GetRequiredService<ScheduleView>();
            Navigate(view, "Таблица расписания");
        }

        public void NavigateToAdd(string initialTab = "Teacher")
        {
            var view = _serviceProvider.GetRequiredService<AddDataView>();
            if (view.DataContext is AddDataViewModel vm)
                vm.CurrentTab = initialTab;
            Navigate(view, "Добавление данных");
        }

        public void NavigateToEdit(string initialTab = "Teacher")
        {
            var view = _serviceProvider.GetRequiredService<EditDataView>();
            if (view.DataContext is EditDataViewModel vm)
                vm.CurrentTab = initialTab;
            Navigate(view, "Редактирование данных");
        }

        public void NavigateToDev()
        {
            var view = _serviceProvider.GetRequiredService<DevView>();
            Navigate(view, "Окно разработчика");
        }

        public void NavigateToSettings()
        {
            var view = _serviceProvider.GetRequiredService<SettingsView>();
            Navigate(view, "Настройки");
        }
    }
}