using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExamSheduleDesign.ViewModels
{
    public partial class AddDataViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;

        // Текущая вкладка: "Teacher", "Discipline", "Group"
        [ObservableProperty]
        private string _currentTab = "Teacher";

        // Поля для добавления преподавателя
        [ObservableProperty]
        private string _newTeacherFullName = "";

        // Поля для добавления дисциплины
        [ObservableProperty]
        private string _newDisciplineName = "";

        // Поля для добавления группы
        [ObservableProperty]
        private string _newGroupName = "";

        [ObservableProperty]
        private string _newGroupDepartment = "Информатика";

        public List<string> DepartmentOptions { get; } = new() { "Информатика", "Экономика", "Гуманитарное" };

        // Коллекции для таблиц
        public ObservableCollection<Teacher> Teachers { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

        public AddDataViewModel(IDataService dataService, INotificationService notificationService)
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