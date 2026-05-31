using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExamSheduleDesign.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

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

        public ObservableCollection<Teacher> Teachers { get; }
        public ObservableCollection<Discipline> Disciplines { get; }
        public ObservableCollection<Group> Groups { get; }
        public List<string> TimeOptions { get; } = new List<string>
        {
            "9:00", "10:40", "11:00", "13:00", "13:30", "14:35", "15:00", "16:20"
        };
        public List<string> TypeOptions { get; } = new List<string> { "Экзамен", "Консультация" };
        public List<Teacher> TeacherListForCombo { get; }

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Teachers = new ObservableCollection<Teacher>(_dataService.GetTeachers());
            Disciplines = new ObservableCollection<Discipline>(_dataService.GetDisciplines());
            Groups = new ObservableCollection<Group>(_dataService.GetGroups());

            var teacherList = _dataService.GetTeachers();
            TeacherListForCombo = new List<Teacher> { null };
            TeacherListForCombo.AddRange(teacherList);
        }

        [RelayCommand]
        private void AddExam()
        {
            if (SelectedTeacher1 == null || SelectedDiscipline == null || SelectedGroup == null)
            {
                MessageBox.Show("Заполните обязательные поля: преподаватель, дисциплина, группа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var exam = new Exam
            {
                Date = SelectedDate,
                Time = SelectedTime,
                Type = SelectedType,
                Teacher1 = SelectedTeacher1,
                Teacher2 = SelectedTeacher2, // может быть null
                Discipline = SelectedDiscipline,
                Group = SelectedGroup,
                Classroom = Classroom
            };

            _dataService.AddExam(exam);
            MessageBox.Show($"Экзамен добавлен!\n{SelectedDiscipline.Name}, {SelectedGroup.Name}, {SelectedDate:dd.MM.yyyy}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Очистка формы (опционально)
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
            if (Application.Current.MainWindow.DataContext is MainWindowViewModel mainVm)
            {
                mainVm.NavigateToScheduleCommand.Execute(null);
            }
        }

        // Заглушки для остальных кнопок (можно потом перенести в отдельные команды)
        public void AddTeacher() => MessageBox.Show("Добавление преподавателя (будет реализовано)");
        public void AddDiscipline() => MessageBox.Show("Добавление дисциплины (будет реализовано)");
        public void AddGroup() => MessageBox.Show("Добавление группы (будет реализовано)");
        public void EditTeachers() => MessageBox.Show("Редактирование преподавателей (будет реализовано)");
        public void EditDisciplines() => MessageBox.Show("Редактирование дисциплин (будет реализовано)");
        public void EditGroups() => MessageBox.Show("Редактирование групп (будет реализовано)");
        public void SaveBuffer() => MessageBox.Show("Сохранение буфера (будет реализовано)");
        public void LoadBuffer() => MessageBox.Show("Загрузка буфера (будет реализовано)");
        public void ClearBuffer() => MessageBox.Show("Очистка буфера (будет реализовано)");
    }
}