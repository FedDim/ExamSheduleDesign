using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public interface IDocumentGenerator
    {
        Task GenerateAllDocumentsAsync(string folderPath, CancellationToken cancellationToken = default, IProgress<string> progress = null);
    }
}