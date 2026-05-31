using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Views;
using System.Windows.Controls;

namespace ExamSheduleDesign.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserControl _currentPage;

        [ObservableProperty]
        private string _breadcrumb = "Главная";

        [ObservableProperty]
        private bool _isDevMode = false;

        [ObservableProperty]
        private string _currentPageTag = "Main";

        // Храним последнюю выбранную вкладку для Add и Edit
        private string _addDataTab = "Teacher";
        private string _editDataTab = "Teacher";

        public MainWindowViewModel()
        {
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
            Navigate("Main", new MainView(), "Главная");
        }

        [RelayCommand]
        private void NavigateToSchedule()
        {
            Navigate("Schedule", new ScheduleView(), "Таблица расписания");
        }

        [RelayCommand]
        private void NavigateToAdd()
        {
            var view = new AddDataView();
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
            var view = new EditDataView();
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
            Navigate("Dev", new DevView(), "Окно разработчика");
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            Navigate("Settings", new SettingsView(), "Настройки");
        }
    }
}