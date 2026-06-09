using ExamSheduleDesign.Controls;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public class WordDocumentGenerator : IDocumentGenerator
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly IAppLogger _logger;

        public WordDocumentGenerator(IDataService dataService, INotificationService notificationService, IAppLogger logger)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task GenerateAllDocumentsAsync(string folderPath)
        {
            try
            {
                var exams = await _dataService.GetExamsAsync();
                if (exams == null || exams.Count == 0)
                {
                    _notificationService.Show("Нет экзаменов для генерации документов.", NotificationType.Warning);
                    return;
                }

                // Запускаем генерацию в отдельном STA-потоке (требуется для COM Interop)
                await Task.Run(() =>
                {
                    var wordHelper = new WordHelper(exams);
                    wordHelper.CreateAllDocuments(folderPath);
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при генерации документов", ex);
                _notificationService.Show($"Ошибка при создании документов: {ex.Message}", NotificationType.Error);
                throw;
            }
        }
    }
}