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
    public partial class EditDataViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private string _currentTab = "Teacher";

        [ObservableProperty]
        private object? _selectedTeacher;

        [ObservableProperty]
        private object? _selectedDiscipline;

        [ObservableProperty]
        private object? _selectedGroup;

        [ObservableProperty]
        private bool _canDeleteTeacher;

        [ObservableProperty]
        private bool _canDeleteDiscipline;

        [ObservableProperty]
        private bool _canDeleteGroup;

        public ObservableCollection<Teacher> Teachers { get; set; } = new();
        public ObservableCollection<Discipline> Disciplines { get; set; } = new();
        public ObservableCollection<Group> Groups { get; set; } = new();

        public EditDataViewModel(IDataService dataService, INotificationService notificationService, ISettingsService settingsService)
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
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка загрузки справочников: {ex.Message}", NotificationType.Error);
            }
        }

        partial void OnSelectedTeacherChanged(object? value) => CanDeleteTeacher = value != null;
        partial void OnSelectedDisciplineChanged(object? value) => CanDeleteDiscipline = value != null;
        partial void OnSelectedGroupChanged(object? value) => CanDeleteGroup = value != null;

        [RelayCommand]
        private void SwitchTab(string tab) => CurrentTab = tab;

        [RelayCommand]
        private async Task SaveTeachersAsync()
        {
            // Проверка на пустые имена
            foreach (var teacher in Teachers)
            {
                if (string.IsNullOrWhiteSpace(teacher.Name))
                {
                    _notificationService.Show("Имя преподавателя не может быть пустым.", NotificationType.Warning);
                    return;
                }
            }

            // Проверка дубликатов (регистронезависимо)
            var duplicateNames = Teachers
                .GroupBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateNames.Any())
            {
                _notificationService.Show($"Обнаружены дубликаты преподавателей: {string.Join(", ", duplicateNames)}", NotificationType.Warning);
                return;
            }

            try
            {
                await _dataService.UpdateTeachersAsync(Teachers);
                _notificationService.Show("Изменения преподавателей сохранены.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка при сохранении преподавателей: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private async Task SaveDisciplinesAsync()
        {
            // Проверка на пустые названия
            foreach (var discipline in Disciplines)
            {
                if (string.IsNullOrWhiteSpace(discipline.FullName))
                {
                    _notificationService.Show("Полное название дисциплины не может быть пустым.", NotificationType.Warning);
                    return;
                }
            }

            // Проверка дубликатов (регистронезависимо)
            var duplicateNames = Disciplines
                .GroupBy(d => d.FullName, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateNames.Any())
            {
                _notificationService.Show($"Обнаружены дубликаты дисциплин: {string.Join(", ", duplicateNames)}", NotificationType.Warning);
                return;
            }

            try
            {
                await _dataService.UpdateDisciplinesAsync(Disciplines);
                _notificationService.Show("Изменения дисциплин сохранены.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка при сохранении дисциплин: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        private async Task SaveGroupsAsync()
        {
            // Проверка на пустые названия
            foreach (var group in Groups)
            {
                if (string.IsNullOrWhiteSpace(group.Name))
                {
                    _notificationService.Show("Название группы не может быть пустым.", NotificationType.Warning);
                    return;
                }
            }

            // Проверка дубликатов (регистронезависимо)
            var duplicateNames = Groups
                .GroupBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateNames.Any())
            {
                _notificationService.Show($"Обнаружены дубликаты групп: {string.Join(", ", duplicateNames)}", NotificationType.Warning);
                return;
            }

            // Проверка формата группы (если валидация включена)
            if (_settingsService.IsGroupValidationEnabled)
            {
                var invalidGroups = Groups
                    .Where(g => !IsGroupNameValid(g.Name))
                    .Select(g => g.Name)
                    .ToList();
                if (invalidGroups.Any())
                {
                    _notificationService.Show($"Названия групп не соответствуют формату (буквы-цифры): {string.Join(", ", invalidGroups)}", NotificationType.Warning);
                    return;
                }
            }

            try
            {
                await _dataService.UpdateGroupsAsync(Groups);
                _notificationService.Show("Изменения групп сохранены.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка при сохранении групп: {ex.Message}", NotificationType.Error);
            }
        }

        private bool IsGroupNameValid(string name)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[А-ЯЁ]{2}-\d{2}$");
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

        [RelayCommand]
        private async Task DeleteCurrentAsync()
        {
            switch (CurrentTab)
            {
                case "Teacher":
                    if (SelectedTeacher is Teacher teacher)
                    {
                        await _dataService.DeleteTeacherAsync(teacher.Id);
                        await LoadDataAsync();
                    }
                    break;
                case "Discipline":
                    if (SelectedDiscipline is Discipline discipline)
                    {
                        await _dataService.DeleteDisciplineAsync(discipline.Id);
                        await LoadDataAsync();
                    }
                    break;
                case "Group":
                    if (SelectedGroup is Group group)
                    {
                        await _dataService.DeleteGroupAsync(group.Id);
                        await LoadDataAsync();
                    }
                    break;
            }
        }
    }
}