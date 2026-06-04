using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public class WordDocumentGenerator : IDocumentGenerator
    {
        private readonly IDataService _dataService;

        public WordDocumentGenerator(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task GenerateAllDocumentsAsync(string folderPath)
        {
            //var exams = await _dataService.GetExamsAsync();
            //var wordHelper = new WordHelper(exams);
            //wordHelper.CreateAllDocuments(folderPath);
        }
    }
}