using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Controls;
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
        private int _totalCount;

        [ObservableProperty]
        private bool _canEdit;

        [ObservableProperty]
        private string _sortColumn = "Преподаватель 1";

        [ObservableProperty]
        private bool _isSortAscending = true;

        [ObservableProperty]
        private Exam? _selectedExam;

        [ObservableProperty]
        private bool? _selectAllState;

        public List<string> SortColumns { get; } = new()
        {
            "Преподаватель 1", "Преподаватель 2", "Дата", "Дисциплина", "Группа", "Время", "Аудитория", "Тип"
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
                UpdateCounts();
                ApplySort();

                SelectedExam = _savedExamId.HasValue ? Exams.FirstOrDefault(e => e.Id == _savedExamId.Value) : null;
                UpdateSelectAllState();
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка загрузки экзаменов: {ex.Message}", NotificationType.Error);
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
            {
                UpdateCounts();
                UpdateSelectAllState();
            }
        }

        private void UpdateCounts()
        {
            SelectedCount = Exams?.Count(e => e.IsSelected) ?? 0;
            TotalCount = Exams?.Count ?? 0;
            CanEdit = SelectedCount == 1;
        }

        private void UpdateSelectAllState()
        {
            if (Exams == null || Exams.Count == 0)
                SelectAllState = false;
            else if (Exams.All(e => e.IsSelected))
                SelectAllState = true;
            else if (Exams.Any(e => e.IsSelected))
                SelectAllState = null;
            else
                SelectAllState = false;
        }

        [RelayCommand]
        private void SelectAll()
        {
            bool newState = SelectAllState != true;
            foreach (var exam in Exams)
                exam.IsSelected = newState;
        }

        partial void OnSortColumnChanged(string value) => ApplySort();
        partial void OnIsSortAscendingChanged(bool value) => ApplySort();

        private void ApplySort()
        {
            if (Exams == null || Exams.Count == 0) return;

            var sorted = SortColumn switch
            {
                "Преподаватель 1" => IsSortAscending ? Exams.OrderBy(e => e.Teacher1?.Name) : Exams.OrderByDescending(e => e.Teacher1?.Name),
                "Преподаватель 2" => IsSortAscending ? Exams.OrderBy(e => e.Teacher2?.Name) : Exams.OrderByDescending(e => e.Teacher2?.Name),
                "Дата" => IsSortAscending ? Exams.OrderBy(e => e.Date) : Exams.OrderByDescending(e => e.Date),
                "Дисциплина" => IsSortAscending ? Exams.OrderBy(e => e.Discipline?.FullName) : Exams.OrderByDescending(e => e.Discipline?.FullName),
                "Группа" => IsSortAscending ? Exams.OrderBy(e => e.Group?.Name) : Exams.OrderByDescending(e => e.Group?.Name),
                "Время" => IsSortAscending ? Exams.OrderBy(e => e.Time) : Exams.OrderByDescending(e => e.Time),
                "Аудитория" => IsSortAscending ? Exams.OrderBy(e => e.Classroom) : Exams.OrderByDescending(e => e.Classroom),
                "Тип" => IsSortAscending ? Exams.OrderBy(e => e.Type) : Exams.OrderByDescending(e => e.Type),
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
                    _notificationService.Show("Документы успешно созданы.", NotificationType.Success);
                }
                catch (Exception ex)
                {
                    _notificationService.Show($"Ошибка при создании документов: {ex.Message}", NotificationType.Error);
                }
            }
        }

        [RelayCommand]
        private async Task EditExamAsync()
        {
            if (SelectedExam == null)
            {
                _notificationService.Show("Выберите экзамен для редактирования.", NotificationType.Warning);
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
                _notificationService.Show("Экзамен успешно обновлён.", NotificationType.Success);
            }
        }
    }
}