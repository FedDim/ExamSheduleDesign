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
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly SqlDataService _sqlDataService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Parse("2026-06-15");

        [ObservableProperty]
        private Teacher _selectedTeacher1;

        [ObservableProperty]
        private Teacher _selectedTeacher2;

        [ObservableProperty]
        private Discipline _selectedDiscipline;

        [ObservableProperty]
        private Group _selectedGroup;

        [ObservableProperty]
        private string _selectedTime = "9:00";

        [ObservableProperty]
        private string _classroom = "301";

        [ObservableProperty]
        private string _selectedType = "Экзамен";

        public ObservableCollection<Teacher> Teachers { get; } = new();
        public ObservableCollection<Discipline> Disciplines { get; } = new();
        public ObservableCollection<Group> Groups { get; } = new();
        public List<string> TimeOptions { get; } = new() { "9:00", "10:40", "11:00", "13:00", "13:30", "14:35", "15:00", "16:20" };
        public List<string> TypeOptions { get; } = new() { "Экзамен", "Консультация" };
        public List<Teacher> TeacherListForCombo { get; private set; } = new() { null };

        public MainViewModel(IDataService dataService, SqlDataService sqlDataService, INotificationService notificationService)
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

                TeacherListForCombo = new List<Teacher> { null };
                TeacherListForCombo.AddRange(teachers);
                OnPropertyChanged(nameof(TeacherListForCombo));
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка загрузки справочников: {ex.Message}");
            }
        }

        [RelayCommand]
        private void AddExam()
        {
            if (SelectedTeacher1 == null || SelectedDiscipline == null || SelectedGroup == null)
            {
                _notificationService.Show("Заполните обязательные поля: преподаватель, дисциплина, группа.");
                return;
            }

            var exam = new Exam
            {
                Date = SelectedDate,
                Time = SelectedTime,
                Type = SelectedType,
                Teacher1 = SelectedTeacher1,
                Teacher2 = SelectedTeacher2,
                Discipline = SelectedDiscipline,
                Group = SelectedGroup,
                Classroom = Classroom
            };

            _dataService.AddExam(exam);
            _notificationService.Show($"Экзамен добавлен!\n{SelectedDiscipline.FullName}, {SelectedGroup.Name}, {SelectedDate:dd.MM.yyyy}");

            SelectedTeacher1 = null;
            SelectedTeacher2 = null;
            SelectedDiscipline = null;
            SelectedGroup = null;
            Classroom = "301";
            SelectedTime = "9:00";
            SelectedType = "Экзамен";
        }

        [RelayCommand]
        private void ShowSchedule()
        {
            if (App.Current.MainWindow.DataContext is MainWindowViewModel mainVm)
                mainVm.NavigateToScheduleCommand.Execute(null);
        }

        [RelayCommand]
        private void AddTeacher()
        {
            if (App.Current.MainWindow.DataContext is MainWindowViewModel mainVm)
                mainVm.NavigateToAddWithParamCommand.Execute("Teacher");
        }

        [RelayCommand]
        private void AddDiscipline()
        {
            if (App.Current.MainWindow.DataContext is MainWindowViewModel mainVm)
                mainVm.NavigateToAddWithParamCommand.Execute("Discipline");
        }

        [RelayCommand]
        private void AddGroup()
        {
            if (App.Current.MainWindow.DataContext is MainWindowViewModel mainVm)
                mainVm.NavigateToAddWithParamCommand.Execute("Group");
        }

        [RelayCommand]
        private void EditTeachers()
        {
            if (App.Current.MainWindow.DataContext is MainWindowViewModel mainVm)
                mainVm.NavigateToEditWithParamCommand.Execute("Teacher");
        }

        [RelayCommand]
        private void EditDisciplines()
        {
            if (App.Current.MainWindow.DataContext is MainWindowViewModel mainVm)
                mainVm.NavigateToEditWithParamCommand.Execute("Discipline");
        }

        [RelayCommand]
        private void EditGroups()
        {
            if (App.Current.MainWindow.DataContext is MainWindowViewModel mainVm)
                mainVm.NavigateToEditWithParamCommand.Execute("Group");
        }

        public void SaveBuffer() => _notificationService.Show("Сохранение буфера (будет реализовано)");
        public void LoadBuffer() => _notificationService.Show("Загрузка буфера (будет реализовано)");
        public void ClearBuffer() => _notificationService.Show("Очистка буфера (будет реализовано)");
    }
}