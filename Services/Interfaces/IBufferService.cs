using System.Threading.Tasks;

namespace ExamSheduleDesign.Services.Interfaces
{
    public interface IBufferService
    {
        Task SaveBufferAsync();
        Task LoadBufferAsync();
        Task ClearBufferAsync();
        Task<bool> HasBufferAsync();
        Task<int> GetBufferCountAsync();
    }
}