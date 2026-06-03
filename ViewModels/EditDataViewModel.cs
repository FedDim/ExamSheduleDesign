using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ExamSheduleDesign.ViewModels
{
    public partial class EditDataViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly SqlDataService _sqlDataService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string _currentTab = "Teacher";

        public ObservableCollection<Teacher> Teachers { get; set; } = new();
        public ObservableCollection<Discipline> Disciplines { get; set; } = new();
        public ObservableCollection<Group> Groups { get; set; } = new();

        public EditDataViewModel(IDataService dataService, SqlDataService sqlDataService, INotificationService notificationService)
        {
            _dataService = dataService;
            _sqlDataService = sqlDataService;
            _notificationService = notificationService;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var teachers = await _sqlDataService.GetTeachersAsync();
                var disciplines = await _sqlDataService.GetDisciplinesAsync();
                var groups = await _sqlDataService.GetGroupsAsync();

                Teachers.Clear();
                Disciplines.Clear();
                Groups.Clear();

                foreach (var t in teachers) Teachers.Add(t);
                foreach (var d in disciplines) Disciplines.Add(d);
                foreach (var g in groups) Groups.Add(g);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка загрузки справочников: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SwitchTab(string tab) => CurrentTab = tab;

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