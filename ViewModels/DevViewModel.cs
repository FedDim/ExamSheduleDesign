using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ExamSheduleDesign.ViewModels
{
    public partial class DevViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly SqlDataService _sqlDataService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private int _generationCount = 10;

        [ObservableProperty]
        private string _statusText = "Готов к генерации";

        [ObservableProperty]
        private bool _isGenerating;

        [ObservableProperty]
        private ObservableCollection<Exam> _generatedExams = new();

        public DevViewModel(IDataService dataService, SqlDataService sqlDataService, INotificationService notificationService)
        {
            _dataService = dataService;
            _sqlDataService = sqlDataService;
            _notificationService = notificationService;
        }

        public async Task LoadDataAsync()
        {
            await RefreshGeneratedListAsync();
            await UpdateCountsAsync();
        }

        private async Task RefreshGeneratedListAsync()
        {
            var allExams = await _sqlDataService.GetExamsAsync();
            var lastExams = allExams.Skip(Math.Max(0, allExams.Count - 50)).ToList();
            GeneratedExams = new ObservableCollection<Exam>(lastExams);
        }

        private async Task UpdateCountsAsync()
        {
            OnPropertyChanged(nameof(TeachersCount));
            OnPropertyChanged(nameof(DisciplinesCount));
            OnPropertyChanged(nameof(GroupsCount));
        }

        public int TeachersCount => _dataService.GetTeachers().Count;
        public int DisciplinesCount => _dataService.GetDisciplines().Count;
        public int GroupsCount => _dataService.GetGroups().Count;

        [RelayCommand]
        private async Task TestLoadExamsAsync()
        {
            try
            {
                var exams = await _sqlDataService.GetExamsAsync();
                _notificationService.Show($"Загружено экзаменов: {exams.Count}");
            }
            catch (Exception ex)
            {
                _notificationService.Show($"Ошибка: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task GenerateAsync()
        {
            if (IsGenerating) return;
            IsGenerating = true;
            StatusText = "Генерация...";

            await Task.Run(() => _dataService.GenerateExams(GenerationCount));

            await RefreshGeneratedListAsync();
            StatusText = $"Сгенерировано {GenerationCount} экзаменов";
            IsGenerating = false;
        }

        [RelayCommand]
        private async Task ClearGeneratedAsync()
        {
            if (MessageBox.Show("Удалить все сгенерированные экзамены?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _dataService.ClearGeneratedExams();
                await RefreshGeneratedListAsync();
                StatusText = "Сгенерированные экзамены удалены";
            }
        }
    }
}