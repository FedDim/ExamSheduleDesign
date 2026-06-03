using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ExamSheduleDesign.ViewModels
{
    public partial class ScheduleViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<Exam> _exams;

        [ObservableProperty]
        private int _selectedCount;

        [ObservableProperty]
        private string _sortColumn = "Date";

        [ObservableProperty]
        private bool _isSortAscending = true;   // новое имя свойства

        public List<string> SortColumns { get; } = new()
        {
            "Date", "Teacher1", "Teacher2", "Discipline", "Group", "Time", "Classroom", "Type"
        };

        public ICommand SortAscendingCommand { get; }
        public ICommand SortDescendingCommand { get; }

        public ScheduleViewModel(IDataService dataService, INotificationService notificationService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            LoadExams();

            SortAscendingCommand = new RelayCommand(() => { IsSortAscending = true; ApplySort(); });
            SortDescendingCommand = new RelayCommand(() => { IsSortAscending = false; ApplySort(); });
        }

        private void LoadExams()
        {
            Exams = new ObservableCollection<Exam>(_dataService.GetExams());
            UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
        {
            SelectedCount = Exams?.Count(e => e.IsSelected) ?? 0;
        }

        partial void OnSortColumnChanged(string value) => ApplySort();
        partial void OnIsSortAscendingChanged(bool value) => ApplySort();

        private void ApplySort()
        {
            if (Exams == null) return;

            var sorted = SortColumn switch
            {
                "Teacher1" => IsSortAscending ? Exams.OrderBy(e => e.Teacher1?.FullName) : Exams.OrderByDescending(e => e.Teacher1?.FullName),
                "Teacher2" => IsSortAscending ? Exams.OrderBy(e => e.Teacher2?.FullName) : Exams.OrderByDescending(e => e.Teacher2?.FullName),
                "Date" => IsSortAscending ? Exams.OrderBy(e => e.Date) : Exams.OrderByDescending(e => e.Date),
                "Discipline" => IsSortAscending ? Exams.OrderBy(e => e.Discipline?.Name) : Exams.OrderByDescending(e => e.Discipline?.Name),
                "Group" => IsSortAscending ? Exams.OrderBy(e => e.Group?.Name) : Exams.OrderByDescending(e => e.Group?.Name),
                "Time" => IsSortAscending ? Exams.OrderBy(e => e.Time) : Exams.OrderByDescending(e => e.Time),
                "Classroom" => IsSortAscending ? Exams.OrderBy(e => e.Classroom) : Exams.OrderByDescending(e => e.Classroom),
                "Type" => IsSortAscending ? Exams.OrderBy(e => e.Type) : Exams.OrderByDescending(e => e.Type),
                _ => IsSortAscending ? Exams.OrderBy(e => e.Date) : Exams.OrderByDescending(e => e.Date),
            };

            var newCollection = new ObservableCollection<Exam>(sorted);
            Exams.Clear();
            foreach (var exam in newCollection)
                Exams.Add(exam);
        }

        [RelayCommand]
        private void ToggleSelection(Exam exam)
        {
            if (exam != null)
            {
                exam.IsSelected = !exam.IsSelected;
                UpdateSelectedCount();
            }
        }

        [RelayCommand]
        private void DeleteSelected()
        {
            var toDelete = Exams.Where(e => e.IsSelected).ToList();
            if (toDelete.Any())
            {
                _dataService.RemoveExams(toDelete);
                LoadExams();
            }
        }

        [RelayCommand]
        private void CreateScheduleFile()
        {
            _notificationService.Show("Создание файла расписания (будет реализовано позже)");
        }
    }
}