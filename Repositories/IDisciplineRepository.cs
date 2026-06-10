using ExamSheduleDesign.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Repositories
{
    public interface IDisciplineRepository
    {
        Task<List<Discipline>> GetAllAsync();
        Task<Discipline?> GetByIdAsync(int id);
        Task AddAsync(Discipline discipline);
        Task UpdateAsync(Discipline discipline);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string shortName9);
        Task<bool> HasExamsAsync(int id);
    }
}
