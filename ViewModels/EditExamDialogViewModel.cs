using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ExamSheduleDesign.ViewModels
{
    public partial class EditExamDialogViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly Window _dialog;

        public Exam Exam { get; }

        public List<string> TimeOptions { get; } = new()
        {
            "9:00", "10:40", "11:00", "13:00", "13:30", "14:35", "15:00", "16:20"
        };

        public List<string> TypeOptions { get; } = new() { "Экзамен", "Консультация" };

        public ObservableCollection<Teacher> Teachers { get; } = new();
        public ObservableCollection<Discipline> Disciplines { get; } = new();
        public ObservableCollection<Group> Groups { get; } = new();

        public EditExamDialogViewModel(Exam exam, IDataService dataService, Window dialog)
        {
            Exam = exam;
            _dataService = dataService;
            _dialog = dialog;
            _ = LoadDataAsync(); // загружаем списки и обновляем ссылки
        }

        private async Task LoadDataAsync()
        {
            // Загружаем свежие справочники
            var teachers = await _dataService.GetTeachersAsync();
            var disciplines = await _dataService.GetDisciplinesAsync();
            var groups = await _dataService.GetGroupsAsync();

            foreach (var t in teachers) Teachers.Add(t);
            foreach (var d in disciplines) Disciplines.Add(d);
            foreach (var g in groups) Groups.Add(g);

            // Подменяем объекты в Exam на те, что содержатся в загруженных коллекциях
            // (чтобы SelectedItem привязка работала по ссылке)
            if (Exam.Teacher1 != null)
            {
                var newTeacher1 = Teachers.FirstOrDefault(t => t.Id == Exam.Teacher1.Id);
                if (newTeacher1 != null) Exam.Teacher1 = newTeacher1;
            }
            if (Exam.Teacher2 != null)
            {
                var newTeacher2 = Teachers.FirstOrDefault(t => t.Id == Exam.Teacher2.Id);
                if (newTeacher2 != null) Exam.Teacher2 = newTeacher2;
            }
            if (Exam.Discipline != null)
            {
                var newDiscipline = Disciplines.FirstOrDefault(d => d.Id == Exam.Discipline.Id);
                if (newDiscipline != null) Exam.Discipline = newDiscipline;
            }
            if (Exam.Group != null)
            {
                var newGroup = Groups.FirstOrDefault(g => g.Id == Exam.Group.Id);
                if (newGroup != null) Exam.Group = newGroup;
            }

            // Уведомляем UI, что свойства Exam изменились (для обновления привязок)
            OnPropertyChanged(nameof(Exam));
        }

        [RelayCommand]
        private void Save()
        {
            _dialog.DialogResult = true;
            _dialog.Close();
        }

        [RelayCommand]
        private void Cancel()
        {
            _dialog.DialogResult = false;
            _dialog.Close();
        }
    }
}