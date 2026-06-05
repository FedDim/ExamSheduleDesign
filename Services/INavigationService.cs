using System.ComponentModel;
using System.Windows.Controls;

namespace ExamSheduleDesign.Services
{
    public interface INavigationService : INotifyPropertyChanged
    {
        UserControl CurrentPage { get; }
        string Breadcrumb { get; }
        void NavigateToMain();
        void NavigateToSchedule();
        void NavigateToAdd(string initialTab = "Teacher");
        void NavigateToEdit(string initialTab = "Teacher");
        void NavigateToDev();
        void NavigateToSettings();
    }
}