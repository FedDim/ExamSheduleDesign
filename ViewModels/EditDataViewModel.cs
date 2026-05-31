using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExamSheduleDesign.ViewModels
{
    public partial class EditDataViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private string _currentTab = "Teacher";

        public ObservableCollection<Teacher> Teachers { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

        public EditDataViewModel(IDataService dataService)
        {
            _dataService = dataService;
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
            MessageBox.Show("Изменения преподавателей сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void SaveDisciplines()
        {
            _dataService.UpdateDisciplines(Disciplines);
            MessageBox.Show("Изменения дисциплин сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void SaveGroups()
        {
            _dataService.UpdateGroups(Groups);
            MessageBox.Show("Изменения групп сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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