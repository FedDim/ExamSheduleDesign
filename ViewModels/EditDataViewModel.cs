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
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string _currentTab = "Teacher";

        public ObservableCollection<Teacher> Teachers { get; set; } = new();
        public ObservableCollection<Discipline> Disciplines { get; set; } = new();
        public ObservableCollection<Group> Groups { get; set; } = new();

        public EditDataViewModel(IDataService dataService, INotificationService notificationService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var teachers = await _dataService.GetTeachersAsync();
                var disciplines = await _dataService.GetDisciplinesAsync();
                var groups = await _dataService.GetGroupsAsync();

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
        private async Task SaveTeachersAsync()
        {
            await _dataService.UpdateTeachersAsync(Teachers);
            _notificationService.Show("Изменения преподавателей сохранены.");
        }

        [RelayCommand]
        private async Task SaveDisciplinesAsync()
        {
            await _dataService.UpdateDisciplinesAsync(Disciplines);
            _notificationService.Show("Изменения дисциплин сохранены.");
        }

        [RelayCommand]
        private async Task SaveGroupsAsync()
        {
            await _dataService.UpdateGroupsAsync(Groups);
            _notificationService.Show("Изменения групп сохранены.");
        }

        [RelayCommand]
        private async Task SaveCurrentAsync()
        {
            switch (CurrentTab)
            {
                case "Teacher": await SaveTeachersAsync(); break;
                case "Discipline": await SaveDisciplinesAsync(); break;
                case "Group": await SaveGroupsAsync(); break;
            }
        }
    }
}