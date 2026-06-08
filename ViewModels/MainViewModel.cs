using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Controls;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using ExamSheduleDesign.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExamSheduleDesign.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IBufferService _bufferService;

        private int? _savedTeacher1Id;
        private int? _savedTeacher2Id;
        private int? _savedDisciplineId;
        private int? _savedGroupId;

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
        private string _classroom = "";

        [ObservableProperty]
        private string _selectedType = "Экзамен";

        public ObservableCollection<Teacher> Teachers { get; } = new();
        public ObservableCollection<Discipline> Disciplines { get; } = new();
        public ObservableCollection<Group> Groups { get; } = new();
        public List<string> TimeOptions { get; } = new() { "9:00", "10:40", "11:00", "13:00", "13:30", "14:35", "15:00", "16:20" };
        public List<string> TypeOptions { get; } = new() { "Экзамен", "Консультация" };
        public List<Teacher> TeacherListForCombo { get; private set; } = new() { null };

        public MainViewModel(IDataService dataService, INotificationService notificationService,
                             INavigationService navigationService, IBufferService bufferService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            _navigationService = navigationService;
            _bufferService = bufferService;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                _savedTeacher1Id = SelectedTeacher1?.Id;
                _savedTeacher2Id = SelectedTeacher2?.Id;
                _savedDisciplineId = SelectedDiscipline?.Id;
                _savedGroupId = SelectedGroup?.Id;

                var teachers = await _dataService.GetTeachersAsync();
                var disciplines = await _dataService.GetDisciplinesAsync();
                var groups = await _dataService.GetGroupsAsync();

                Teachers.Clear();
                Disciplines.Clear();
                Groups.Clear();

                foreach (var t in teachers) Teachers.Add(t);
                foreach (var d in disciplines) Disciplines.Add(d);
                foreach (var g in groups) Groups.Add(g);

                TeacherListForCombo = new List<Teacher> { null };
                TeacherListForCombo.AddRange(teachers);
                OnPropertyChanged(nameof(TeacherListForCombo));

                SelectedTeacher1 = _savedTeacher1Id.HasValue ? Teachers.FirstOrDefault(t => t.Id == _savedTeacher1Id.Value) : null;
                SelectedTeacher2 = _savedTeacher2Id.HasValue ? Teachers.FirstOrDefault(t => t.Id == _savedTeacher2Id.Value) : null;
                SelectedDiscipline = _savedDisciplineId.HasValue ? Disciplines.FirstOrDefault(d => d.Id == _savedDisciplineId.Value) : null;
                SelectedGroup = _savedGroupId.HasValue ? Groups.FirstOrDefault(g => g.Id == _savedGroupId.Value) : null;
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка загрузки справочников: {ex.Message}", NotificationType.Error);
            }
        }

        partial void OnSelectedTeacher1Changed(Teacher? value)
        {
            if (value != null)
            {
                Classroom = value.Classroom ?? "";
            }
        }

        [RelayCommand]
        private async Task AddExamAsync()
        {
            if (SelectedTeacher1 == null || SelectedDiscipline == null || SelectedGroup == null)
            {
                _notificationService.Show("Заполните обязательные поля: преподаватель, дисциплина, группа.", NotificationType.Warning);
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

            try
            {
                await _dataService.AddExamAsync(exam);
                _notificationService.Show($"Экзамен добавлен!\n{SelectedDiscipline.FullName}, {SelectedGroup.Name}, {SelectedDate:dd.MM.yyyy}", NotificationType.Success);
                // Очистка после успешного добавления
                SelectedTeacher1 = null;
                SelectedTeacher2 = null;
                SelectedDiscipline = null;
                SelectedGroup = null;
                Classroom = "301";
                SelectedTime = "9:00";
                SelectedType = "Экзамен";
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка при добавлении экзамена: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private void ShowSchedule() => _navigationService.NavigateToSchedule();

        [RelayCommand]
        private void AddTeacher() => _navigationService.NavigateToAdd("Teacher");

        [RelayCommand]
        private void AddDiscipline() => _navigationService.NavigateToAdd("Discipline");

        [RelayCommand]
        private void AddGroup() => _navigationService.NavigateToAdd("Group");

        [RelayCommand]
        private void EditTeachers() => _navigationService.NavigateToEdit("Teacher");

        [RelayCommand]
        private void EditDisciplines() => _navigationService.NavigateToEdit("Discipline");

        [RelayCommand]
        private void EditGroups() => _navigationService.NavigateToEdit("Group");

        [RelayCommand]
        private async Task SaveBufferAsync()
        {
            try
            {
                await _bufferService.SaveBufferAsync();
                _notificationService.Show("Буфер сохранён.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка сохранения буфера: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private async Task LoadBufferAsync()
        {
            try
            {
                await _bufferService.LoadBufferAsync();
                _notificationService.Show("Буфер загружен.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка загрузки буфера: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private async Task ClearBufferAsync()
        {
            try
            {
                await _bufferService.ClearBufferAsync();
                _notificationService.Show("Буфер очищен.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка очистки буфера: {ex.Message}", NotificationType.Error);
            }
        }
    }
}