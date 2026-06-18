using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Controls;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExamSheduleDesign.ViewModels
{
    public partial class AddDataViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private string _currentTab = "Teacher";

        // Поля для преподавателя
        [ObservableProperty]
        private string _newTeacherFullName = "";
        [ObservableProperty]
        private string _newTeacherClassroom = "";
        [ObservableProperty]
        private int _newTeacherAcademicBuilding;

        // Поля для дисциплины
        [ObservableProperty]
        private string _newDisciplineFullName = "";
        [ObservableProperty]
        private string _newDisciplineShortName12 = "";
        [ObservableProperty]
        private string _newDisciplineShortName9 = "";
        [ObservableProperty]
        private string _newDisciplineShortName5 = "";

        // Поля для группы
        [ObservableProperty]
        private string _newGroupName = "";
        [ObservableProperty]
        private string _newGroupDepartment = "";

        // Коллекция для списка кафедр
        [ObservableProperty]
        private ObservableCollection<string> _departments = new();

        public ObservableCollection<Teacher> Teachers { get; set; } = new();
        public ObservableCollection<Discipline> Disciplines { get; set; } = new();
        public ObservableCollection<Group> Groups { get; set; } = new();

        public AddDataViewModel(IDataService dataService, INotificationService notificationService, ISettingsService settingsService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            _settingsService = settingsService;
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

                var deptList = groups.Select(g => g.Department).Where(d => !string.IsNullOrEmpty(d)).Distinct().OrderBy(d => d).ToList();
                Departments.Clear();
                foreach (var dept in deptList)
                    Departments.Add(dept);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка загрузки справочников: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private void SwitchTab(string tab) => CurrentTab = tab;

        [RelayCommand]
        private async Task AddTeacherAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTeacherFullName))
            {
                _notificationService.Show("Введите ФИО преподавателя.", NotificationType.Warning);
                return;
            }

            // Проверка дубликата
            if (Teachers.Any(t => string.Equals(t.Name, NewTeacherFullName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _notificationService.Show("Преподаватель с таким именем уже существует.", NotificationType.Warning);
                return;
            }

            var teacher = new Teacher
            {
                Name = NewTeacherFullName.Trim(),
                Classroom = NewTeacherClassroom?.Trim() ?? "",
                AcademicBuilding = NewTeacherAcademicBuilding
            };
            try
            {
                await _dataService.AddTeacherAsync(teacher);
                Teachers.Add(teacher);
                NewTeacherFullName = "";
                NewTeacherClassroom = "";
                NewTeacherAcademicBuilding = 0;
                _notificationService.Show("Преподаватель добавлен.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка при добавлении преподавателя: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private async Task AddDisciplineAsync()
        {
            if (string.IsNullOrWhiteSpace(NewDisciplineFullName))
            {
                _notificationService.Show("Введите полное название дисциплины.", NotificationType.Warning);
                return;
            }

            // Проверка дубликата
            if (Disciplines.Any(d => string.Equals(d.FullName, NewDisciplineFullName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _notificationService.Show("Дисциплина с таким названием уже существует.", NotificationType.Warning);
                return;
            }

            var discipline = new Discipline
            {
                FullName = NewDisciplineFullName.Trim(),
                ShortName12 = NewDisciplineShortName12?.Trim() ?? "",
                ShortName9 = NewDisciplineShortName9?.Trim() ?? "",
                ShortName5 = NewDisciplineShortName5?.Trim() ?? ""
            };
            try
            {
                await _dataService.AddDisciplineAsync(discipline);
                Disciplines.Add(discipline);
                NewDisciplineFullName = "";
                NewDisciplineShortName12 = "";
                NewDisciplineShortName9 = "";
                NewDisciplineShortName5 = "";
                _notificationService.Show("Дисциплина добавлена.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка при добавлении дисциплины: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private async Task AddGroupAsync()
        {
            if (string.IsNullOrWhiteSpace(NewGroupName))
            {
                _notificationService.Show("Введите название группы.", NotificationType.Warning);
                return;
            }

            var groupName = NewGroupName.Trim();

            // Проверка формата (если валидация включена)
            if (_settingsService.IsGroupValidationEnabled && !IsGroupNameValid(groupName))
            {
                _notificationService.Show("Название группы должно соответствовать формату: две заглавные буквы, дефис, две цифры (например, КИ-35).", NotificationType.Warning);
                return;
            }

            // Проверка дубликата
            if (Groups.Any(g => string.Equals(g.Name, groupName, StringComparison.OrdinalIgnoreCase)))
            {
                _notificationService.Show("Группа с таким названием уже существует.", NotificationType.Warning);
                return;
            }

            var group = new Group
            {
                Name = groupName,
                Department = NewGroupDepartment?.Trim() ?? ""
            };
            try
            {
                await _dataService.AddGroupAsync(group);
                Groups.Add(group);
                if (!Departments.Contains(group.Department) && !string.IsNullOrEmpty(group.Department))
                    Departments.Add(group.Department);
                NewGroupName = "";
                NewGroupDepartment = "";
                _notificationService.Show("Группа добавлена.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка при добавлении группы: {ex.Message}", NotificationType.Error);
            }
        }

        // Вспомогательный метод проверки формата группы
        private bool IsGroupNameValid(string name)
        {
            // Формат: две заглавные русские буквы, дефис, две цифры
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[А-ЯЁ]{2}-\d{2}$");
        }

        [RelayCommand]
        private async Task ImportFromExcelAsync()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Выберите Excel файл для импорта"
            };
            if (dialog.ShowDialog() != true) return;

            DataType dataType = CurrentTab switch
            {
                "Teacher" => DataType.TEACHER,
                "Discipline" => DataType.DISCIPLINE,
                "Group" => DataType.GROUP,
                _ => DataType.NULL
            };
            if (dataType == DataType.NULL) return;

            try
            {
                var result = await _dataService.ImportFromExcelAsync(dataType, dialog.FileName);
                if (result.ErrorsCount > 0)
                    _notificationService.Show(result.GetSummary(), NotificationType.Error);
                else if (result.Added > 0 && result.Skipped == 0)
                    _notificationService.Show(result.GetSummary(), NotificationType.Success);
                else
                    _notificationService.Show(result.GetSummary(), NotificationType.Warning);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка импорта: {ex.Message}", NotificationType.Error);
            }
        }
    }
}