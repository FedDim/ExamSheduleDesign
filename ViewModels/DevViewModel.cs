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
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private int _generationCount = 10;

        [ObservableProperty]
        private string _statusText = "Готов к генерации";

        [ObservableProperty]
        private bool _isGenerating;

        [ObservableProperty]
        private ObservableCollection<Exam> _generatedExams = new();

        [ObservableProperty]
        private int _teachersCount;

        [ObservableProperty]
        private int _disciplinesCount;

        [ObservableProperty]
        private int _groupsCount;

        public DevViewModel(IDataService dataService, INotificationService notificationService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
        }

        public async Task LoadDataAsync()
        {
            await RefreshGeneratedListAsync();
            await LoadCountsAsync();
        }

        private async Task RefreshGeneratedListAsync()
        {
            var allExams = await _dataService.GetExamsAsync();
            var lastExams = allExams.Skip(Math.Max(0, allExams.Count - 50)).ToList();
            GeneratedExams = new ObservableCollection<Exam>(lastExams);
        }

        private async Task LoadCountsAsync()
        {
            TeachersCount = (await _dataService.GetTeachersAsync()).Count;
            DisciplinesCount = (await _dataService.GetDisciplinesAsync()).Count;
            GroupsCount = (await _dataService.GetGroupsAsync()).Count;
        }

        [RelayCommand]
        private async Task TestLoadExamsAsync()
        {
            try
            {
                var exams = await _dataService.GetExamsAsync();
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

            await _dataService.GenerateExamsAsync(GenerationCount);
            await RefreshGeneratedListAsync();
            await LoadCountsAsync();
            StatusText = $"Сгенерировано {GenerationCount} экзаменов";
            IsGenerating = false;
        }

        [RelayCommand]
        private async Task ClearGeneratedAsync()
        {
            if (MessageBox.Show("Удалить все сгенерированные экзамены?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await _dataService.ClearGeneratedExamsAsync();
                await RefreshGeneratedListAsync();
                await LoadCountsAsync();
                StatusText = "Сгенерированные экзамены удалены";
            }
        }
    }
}