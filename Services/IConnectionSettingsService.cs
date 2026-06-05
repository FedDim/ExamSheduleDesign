using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public interface IConnectionSettingsService
    {
        string Server { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string DatabaseName { get; set; }

        Task LoadAsync();
        Task SaveAsync();
        void ResetToDefaults();
    }
}