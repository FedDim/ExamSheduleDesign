using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System.Collections.ObjectModel;

namespace ExamSheduleDesign.ViewModels
{
    public partial class EditDataViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string _currentTab = "Teacher";

        public ObservableCollection<Teacher> Teachers { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

        public EditDataViewModel(IDataService dataService, INotificationService notificationService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            LoadData();
        }

        private void LoadData()
        {
            Teachers = new ObservableCollection<Teacher>(_dataService.GetTeachers());
            Disciplines = new ObservableCollection<Discipline>(_dataService.GetDisciplines());
            Groups = new ObservableCollection<Group>(_dataService.GetGroups());
        }

        [RelayCommand]
        private void SwitchTab(string tab)
        {
            CurrentTab = tab;
        }

        [RelayCommand]
        private void SaveTeachers()
        {
            _dataService.UpdateTeachers(Teachers);
            _notificationService.Show("Изменения преподавателей сохранены.");
        }

        [RelayCommand]
        private void SaveDisciplines()
        {
            _dataService.UpdateDisciplines(Disciplines);
            _notificationService.Show("Изменения дисциплин сохранены.");
        }

        [RelayCommand]
        private void SaveGroups()
        {
            _dataService.UpdateGroups(Groups);
            _notificationService.Show("Изменения групп сохранены.");
        }

        // Команда сохранения, которая вызывает нужный метод в зависимости от вкладки
        [RelayCommand]
        private void SaveCurrent()
        {
            switch (CurrentTab)
            {
                case "Teacher": SaveTeachers(); break;
                case "Discipline": SaveDisciplines(); break;
                case "Group": SaveGroups(); break;
            }
        }
    }
}