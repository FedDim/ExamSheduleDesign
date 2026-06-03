using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ExamSheduleDesign.ViewModels
{
    public partial class AddDataViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly SqlDataService _sqlDataService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string _currentTab = "Teacher";

        [ObservableProperty]
        private string _newTeacherFullName = "";

        [ObservableProperty]
        private string _newDisciplineName = "";

        [ObservableProperty]
        private string _newGroupName = "";

        [ObservableProperty]
        private string _newGroupDepartment = "Информатика";

        public List<string> DepartmentOptions { get; } = new() { "Информатика", "Экономика", "Гуманитарное" };

        public ObservableCollection<Teacher> Teachers { get; set; } = new();
        public ObservableCollection<Discipline> Disciplines { get; set; } = new();
        public ObservableCollection<Group> Groups { get; set; } = new();

        public AddDataViewModel(IDataService dataService, SqlDataService sqlDataService, INotificationService notificationService)
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
        private void AddTeacher()
        {
            if (string.IsNullOrWhiteSpace(NewTeacherFullName))
            {
                _notificationService.Show("Введите ФИО преподавателя.");
                return;
            }
            var teacher = new Teacher { Name = NewTeacherFullName.Trim() };
            _dataService.AddTeacher(teacher);
            Teachers.Add(teacher);
            NewTeacherFullName = "";
            _notificationService.Show("Преподаватель добавлен.");
        }

        [RelayCommand]
        private void AddDiscipline()
        {
            if (string.IsNullOrWhiteSpace(NewDisciplineName))
            {
                _notificationService.Show("Введите название дисциплины.");
                return;
            }
            var discipline = new Discipline { FullName = NewDisciplineName.Trim() };
            _dataService.AddDiscipline(discipline);
            Disciplines.Add(discipline);
            NewDisciplineName = "";
            _notificationService.Show("Дисциплина добавлена.");
        }

        [RelayCommand]
        private void AddGroup()
        {
            if (string.IsNullOrWhiteSpace(NewGroupName))
            {
                _notificationService.Show("Введите название группы.");
                return;
            }
            var group = new Group { Name = NewGroupName.Trim(), Department = NewGroupDepartment };
            _dataService.AddGroup(group);
            Groups.Add(group);
            NewGroupName = "";
            NewGroupDepartment = "Информатика";
            _notificationService.Show("Группа добавлена.");
        }
    }
}