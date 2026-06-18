using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public interface ISettingsService
    {
        bool IsGroupValidationEnabled { get; set; }
        Task LoadAsync();
        Task SaveAsync();
    }
}