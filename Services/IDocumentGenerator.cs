using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public interface IDocumentGenerator
    {
        Task GenerateAllDocumentsAsync(string folderPath);
    }
}