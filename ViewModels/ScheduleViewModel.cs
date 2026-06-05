using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ExamSheduleDesign.ViewModels
{
    public partial class ScheduleViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly IDocumentGenerator _documentGenerator;
        private int? _savedExamId;

        [ObservableProperty]
        private ObservableCollection<Exam> _exams = new();

        [ObservableProperty]
        private int _selectedCount;

        [ObservableProperty]
        private bool _canEdit;   // true только если выбран ровно один экзамен

        [ObservableProperty]
        private string _sortColumn = "Date";

        [ObservableProperty]
        private bool _isSortAscending = true;

        [ObservableProperty]
        private Exam? _selectedExam;

        public List<string> SortColumns { get; } = new()
        {
            "Date", "Teacher1", "Teacher2", "Discipline", "Group", "Time", "Classroom", "Type"
        };

        public ICommand SortAscendingCommand { get; }
        public ICommand SortDescendingCommand { get; }

        public ScheduleViewModel(IDataService dataService, INotificationService notificationService, IDocumentGenerator documentGenerator)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            _documentGenerator = documentGenerator;

            SortAscendingCommand = new RelayCommand(() => { IsSortAscending = true; ApplySort(); });
            SortDescendingCommand = new RelayCommand(() => { IsSortAscending = false; ApplySort(); });
        }

        public async Task LoadExamsAsync()
        {
            try
            {
                _savedExamId = SelectedExam?.Id;

                var exams = await _dataService.GetExamsAsync();
                UnsubscribeExams();
                Exams = new ObservableCollection<Exam>(exams);
                SubscribeExams();
                UpdateSelectedCount();
                ApplySort();

                SelectedExam = _savedExamId.HasValue ? Exams.FirstOrDefault(e => e.Id == _savedExamId.Value) : null;
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка загрузки экзаменов: {ex.Message}");
            }
        }

        private void SubscribeExams()
        {
            foreach (var exam in Exams)
                exam.PropertyChanged += OnExamPropertyChanged;
        }

        private void UnsubscribeExams()
        {
            foreach (var exam in Exams)
                exam.PropertyChanged -= OnExamPropertyChanged;
        }

        private void OnExamPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Exam.IsSelected))
                UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
        {
            SelectedCount = Exams?.Count(e => e.IsSelected) ?? 0;
            CanEdit = SelectedCount == 1;
        }

        partial void OnSortColumnChanged(string value) => ApplySort();
        partial void OnIsSortAscendingChanged(bool value) => ApplySort();

        private void ApplySort()
        {
            if (Exams == null || Exams.Count == 0) return;

            var sorted = SortColumn switch
            {
                "Teacher1" => IsSortAscending ? Exams.OrderBy(e => e.Teacher1?.Name) : Exams.OrderByDescending(e => e.Teacher1?.Name),
                "Teacher2" => IsSortAscending ? Exams.OrderBy(e => e.Teacher2?.Name) : Exams.OrderByDescending(e => e.Teacher2?.Name),
                "Date" => IsSortAscending ? Exams.OrderBy(e => e.Date) : Exams.OrderByDescending(e => e.Date),
                "Discipline" => IsSortAscending ? Exams.OrderBy(e => e.Discipline?.FullName) : Exams.OrderByDescending(e => e.Discipline?.FullName),
                "Group" => IsSortAscending ? Exams.OrderBy(e => e.Group?.Name) : Exams.OrderByDescending(e => e.Group?.Name),
                "Time" => IsSortAscending ? Exams.OrderBy(e => e.Time) : Exams.OrderByDescending(e => e.Time),
                "Classroom" => IsSortAscending ? Exams.OrderBy(e => e.Classroom) : Exams.OrderByDescending(e => e.Classroom),
                "Type" => IsSortAscending ? Exams.OrderBy(e => e.Type) : Exams.OrderByDescending(e => e.Type),
                _ => IsSortAscending ? Exams.OrderBy(e => e.Date) : Exams.OrderByDescending(e => e.Date),
            };

            var newCollection = new ObservableCollection<Exam>(sorted);
            UnsubscribeExams();
            Exams.Clear();
            foreach (var exam in newCollection)
                Exams.Add(exam);
            SubscribeExams();
        }

        [RelayCommand]
        private void ToggleSelection(Exam exam)
        {
            if (exam != null)
                exam.IsSelected = !exam.IsSelected;
        }

        [RelayCommand]
        private async Task DeleteSelectedAsync()
        {
            var toDelete = Exams.Where(e => e.IsSelected).ToList();
            if (toDelete.Any())
            {
                await _dataService.RemoveExamsAsync(toDelete);
                await LoadExamsAsync();
            }
        }

        [RelayCommand]
        private async Task CreateScheduleFileAsync()
        {
            using var dialog = new FolderBrowserDialog();
            dialog.Description = "Выберите папку для сохранения документов";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _documentGenerator.GenerateAllDocumentsAsync(dialog.SelectedPath);
                    _notificationService.Show("Документы успешно созданы.");
                }
                catch (Exception ex)
                {
                    _notificationService.Show($"Ошибка при создании документов: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private async Task EditExamAsync()
        {
            if (SelectedExam == null)
            {
                _notificationService.Show("Выберите экзамен для редактирования.");
                return;
            }

            var editExam = new Exam
            {
                Id = SelectedExam.Id,
                Date = SelectedExam.Date,
                Time = SelectedExam.Time,
                Type = SelectedExam.Type,
                Teacher1 = SelectedExam.Teacher1,
                Teacher2 = SelectedExam.Teacher2,
                Discipline = SelectedExam.Discipline,
                Group = SelectedExam.Group,
                Classroom = SelectedExam.Classroom,
                IsSelected = SelectedExam.IsSelected
            };

            var dialog = new Views.EditExamDialog(editExam, _dataService);
            if (dialog.ShowDialog() == true)
            {
                await _dataService.UpdateExamAsync(editExam);
                var index = Exams.IndexOf(SelectedExam);
                if (index >= 0)
                {
                    Exams[index] = editExam;
                    SelectedExam = editExam;
                }
                await LoadExamsAsync();
                _notificationService.Show("Экзамен успешно обновлён.");
            }
        }
    }
}