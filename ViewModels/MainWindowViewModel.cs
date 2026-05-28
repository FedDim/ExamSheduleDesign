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

        public MainWindowViewModel()
        {
            _currentPage = new MainView();
        }

        [RelayCommand] private void NavigateToMain() => CurrentPage = new MainView();
        [RelayCommand] private void NavigateToSchedule() => CurrentPage = new ScheduleView();
        [RelayCommand] private void NavigateToAdd() => CurrentPage = new AddDataView();
        [RelayCommand] private void NavigateToEdit() => CurrentPage = new EditDataView();
        [RelayCommand] private void NavigateToDev() => CurrentPage = new DevView();
    }
}
